using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProjetoIs.Models;

namespace ProjetoIs.Controllers
{
    [RoutePrefix("api/somiod/{applicationName}")]
    public class containerController : ApiController
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ProjetoIs.Properties.Settings.ConnectionString"].ConnectionString;

        #region getAll
        //Aqui estava a causar conflito com o outro Get na application. Este vai buscar todos os containers, independentemente da app.
        //Este busca todos os containers que existem na BD.
      
        public List<String> Get()
        {
                List<string> pathsApplicacion = new List<string>();
            try
            {

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"SELECT [resource-name],[application-resource-name] FROM container";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string container_name = reader["resource-name"].ToString();
                            string app_name = reader["application-resource-name"].ToString();
                            pathsApplicacion.Add($"/api/somiod/{app_name}/{container_name}");
                        }
                    }
                }

                return pathsApplicacion;
            }
            catch (Exception ex)
            {
                pathsApplicacion.Add(ex.ToString());
                return pathsApplicacion;
            }
        }
        #endregion
        
        #region get
        [HttpGet]
        [Route("{containerName}")]
public IHttpActionResult GetContainer(string applicationName, string containerName)
{
    
    IEnumerable<string> headers;
    if (!Request.Headers.TryGetValues("somiod-discovery", out headers))
    {
        
        try
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"SELECT * 
                                 FROM container 
                                 WHERE [resource-name] = @resourceName";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@resourceName", containerName);

                    using (var reader = cmd.ExecuteReader())
                    {
                        container containerGet = null;

                        if (reader.Read())
                        {
                            containerGet = new container
                            {
                                ResourceName           = (string)reader["resource-name"],
                                ResType                = (string)reader["res-type"],
                                CreationDatetime       = (DateTime)reader["creation-datetime"],
                                ApplicationResourceName = (string)reader["application-resource-name"]
                            };
                        }

                        if (containerGet != null)
                            return Ok(containerGet);

                        return NotFound();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return InternalServerError(ex);
        }
    }

    
    string resType = headers.FirstOrDefault()?.ToLowerInvariant();
    if (resType == "content-instance")
    {
        
        var pathsCi = new List<string>();

        try
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT [resource-name] 
                    FROM [content-instance]
                    WHERE [container-resource-name] = @container";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@container", containerName);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string ci = reader["resource-name"].ToString();
                            pathsCi.Add($"/api/somiod/{applicationName}/{containerName}/{ci}");
                        }
                    }
                }
            }

            return Ok(pathsCi);
        }
        catch (SqlException ex)
        {
            return InternalServerError(ex);
        }
    }

    // header existe mas não é um tipo que tratamos
    return BadRequest("Unknown somiod-discovery type");
}

        #endregion

        #region post content-instance

        [HttpPost]
        [Route("{containerName}")]
        public IHttpActionResult PostContentInstance( string containerName, [FromBody] content_instance ci)
        {
            if (ci == null || string.IsNullOrWhiteSpace(containerName))
                return BadRequest("Invalid content-instance data");

            ci.ResType = "content-instance";
            ci.CreationDatetime = DateTime.UtcNow;
            ci.ContainerResourceName = containerName;

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // 1) Verificar se o container existe e pertence à app
                    using (var checkContainerCmd = new SqlCommand(
                               @"SELECT COUNT(*) 
                         FROM container 
                         WHERE [resource-name] = @cont 
                           ", conn))
                    {
                        checkContainerCmd.Parameters.AddWithValue("@cont", containerName);
                        
                        int contExists = (int)checkContainerCmd.ExecuteScalar();
                        if (contExists == 0)
                            return NotFound();  // container não existe / não é dessa app
                    }

                    // 2) Garantir que o resource-name é único
                    if (string.IsNullOrWhiteSpace(ci.ResourceName))
                    {
                        ci.ResourceName = ci.CreationDatetime.ToString("yyyyMMdd_HHmmss_fff");
                    }
                    else
                    {
                        using (var checkNameCmd = new SqlCommand(
                                   "SELECT COUNT(*) FROM [content-instance] WHERE [resource-name] = @name",
                                   conn))
                        {
                            checkNameCmd.Parameters.AddWithValue("@name", ci.ResourceName);
                            int nameExists = (int)checkNameCmd.ExecuteScalar();
                            if (nameExists > 0)
                                ci.ResourceName = ci.CreationDatetime.ToString("yyyyMMdd_HHmmss_fff");
                        }
                    }

                    // 3) Inserir a content-instance completa
                    string insertQuery = @"
                INSERT INTO [content-instance]
                    ([resource-name], [creation-datetime], [container-resource-name],
                     [res-type], [content-type], [content])    
                VALUES
                    (@resourceName, @creationDatetime, @containerResourceName,
                     @resType, @contentType, @content)";

                    using (var cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@resourceName", ci.ResourceName);
                        cmd.Parameters.AddWithValue("@creationDatetime", ci.CreationDatetime);
                        cmd.Parameters.AddWithValue("@containerResourceName", ci.ContainerResourceName);
                        cmd.Parameters.AddWithValue("@resType", ci.ResType);
                        cmd.Parameters.AddWithValue("@contentType", (object)ci.ContentType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@content", (object)ci.Content ?? DBNull.Value);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0)
                            return InternalServerError();
                    }
                }

                var location = $"/api/somiod/{containerName}/{ci.ResourceName}";
                return Created(location, ci);
            }
            catch (SqlException ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion

        #region PUT
        [HttpPut]
        [Route("{resourceName}")]
        //Perguntar se esta bem assim
        public IHttpActionResult Put(string applicationName, string resourceName, [FromBody] container containerPut)
        {
            if (string.IsNullOrWhiteSpace(resourceName) || containerPut == null)
            {
                return BadRequest("check the resource name and the new container data");
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                UPDATE container 
                SET [application-resource-name] = @application_resource_name 
                WHERE [resource-name] = @resourceName
                  AND [application-resource-name] = @applicationName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@application_resource_name", containerPut.ApplicationResourceName);
                        cmd.Parameters.AddWithValue("@resourceName", resourceName);
                        cmd.Parameters.AddWithValue("@applicationName", applicationName);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0)
                            return NotFound();
                    }
                }

                // só para que a resposta venha “bonita”
                containerPut.ResType = "container";
                containerPut.ResourceName = resourceName;

                return Ok(containerPut);
            }
            catch (SqlException ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion

        #region Delete

        [HttpDelete]
        [Route("{resourceName}")]
        public IHttpActionResult Delete(string resourceName)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
                return BadRequest("Missing resource name");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"DELETE FROM container WHERE [resource-name] = @resourceName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@resourceName", resourceName);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0)
                            return NotFound();
                    }
                }

                return Ok($"Container '{resourceName}' deleted successfully.");
            }
            catch (SqlException ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion
    }
}
