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
        public IHttpActionResult GetContainer(string applicationName,string containerName)
        {
            IEnumerable<string> headers;
            if (!Request.Headers.TryGetValues("somiod-discovery", out headers)) // verificar se somiod-discovery esta presente
            {
                try // nao esta presente -> dar return a um container
                {
                    using (var conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string query = @"SELECT * FROM container WHERE [resource-name] = @containerName";

                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@containerName", containerName);

                            using (var reader = cmd.ExecuteReader())
                            {
                                container containerGet = null;


                                if (reader.Read())
                                {
                                    containerGet = new container
                                    {
                                        ResourceName = (string)reader["resource-name"],
                                        ResType = (string)reader["res-type"],
                                        CreationDatetime = (DateTime)reader["creation-datetime"],
                                        ApplicationResourceName = (string)reader["application-resource-name"]
                                    };
                                }

                                if (containerGet != null)
                                {
                                    return Ok(new { containerGet.ResourceName, containerGet.ResType, containerGet.CreationDatetime, containerGet.ApplicationResourceName, });
                                }

                                return NotFound();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error on getting the application: {ex.Message}");
                    return InternalServerError(ex);
                }
            }
            else {
                string resType = headers.FirstOrDefault();
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
                    /*
                     if (resType == "content-instance"){
                    }
                     */
                    catch (SqlException ex)
                    {
                        return InternalServerError(ex);
                    }
                }
                return BadRequest("Unknown somiod-discovery type");
            }    
        }
        #endregion
        
        #region post content-instance 
        [HttpPost]
        [Route("{containerName}")] 
        public IHttpActionResult Post(string containerName, [FromBody] content_instance cont_instance)
        {
            if (string.IsNullOrWhiteSpace(containerName) || cont_instance == null || string.IsNullOrWhiteSpace(cont_instance.ResourceName))
            {
                return BadRequest("Missing required field: resource-name or invalid container data");
            }

            cont_instance.ResType = "content-instance";
            cont_instance.CreationDatetime = DateTime.UtcNow;
            cont_instance.ContainerResourceName = containerName;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // 1) Verificar se a container existe
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM container WHERE [resource-name] = @container",
                        conn))
                    {
                        cmd.Parameters.AddWithValue("@container", containerName);
                        int exists = (int)cmd.ExecuteScalar();

                        if (exists == 0)
                            return NotFound(); // container não existe
                    }

                    // 2) Verificar se content-instance ja existe (nome e unico)
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM [content-instance] WHERE [resource-name] = @c",
                        conn))
                    {
                        cmd.Parameters.AddWithValue("@c", cont_instance.ResourceName);
                        int exists = (int)cmd.ExecuteScalar();

                        if (exists > 0)
                        {
                            string uniqueName = cont_instance.CreationDatetime.ToString("yyyyMMdd_HHmmss_fff");

                            string insertQuery = @"INSERT INTO [content-instance] ([resource-name], [res-type], [creation-datetime], [container-resource-name], [content-type], [content]) VALUES (@resourceName, @resType, @creationDatetime, @containerName, @contentType, @content)";

                            using (SqlCommand command = new SqlCommand(insertQuery, conn))
                            {
                                command.Parameters.AddWithValue("@resourceName", uniqueName);
                                command.Parameters.AddWithValue("@resType", cont_instance.ResType);
                                command.Parameters.AddWithValue("@creationDatetime", cont_instance.CreationDatetime);
                                command.Parameters.AddWithValue("@containerName", cont_instance.ContainerResourceName);
                                command.Parameters.AddWithValue("@contentType", cont_instance.ContentType);
                                command.Parameters.AddWithValue("@content", cont_instance.Content);
                                cont_instance.ResourceName = uniqueName;

                                int rows = command.ExecuteNonQuery();
                                if (rows == 0)
                                    return InternalServerError();
                            }
                        }
                        else
                        {
                            // 3) Inserir cont_instance
                            string insertQuery = @"INSERT INTO [content-instance] ([resource-name], [res-type], [creation-datetime], [container-resource-name], [content-type], [content]) VALUES (@resourceName, @resType, @creationDatetime, @containerName, @contentType, @content)";

                            using (SqlCommand command = new SqlCommand(insertQuery, conn))
                            {
                                command.Parameters.AddWithValue("@resourceName", cont_instance.ResourceName);
                                command.Parameters.AddWithValue("@resType", cont_instance.ResType);
                                command.Parameters.AddWithValue("@creationDatetime", cont_instance.CreationDatetime);
                                command.Parameters.AddWithValue("@containerName", cont_instance.ContainerResourceName);
                                command.Parameters.AddWithValue("@contentType", cont_instance.ContentType);
                                command.Parameters.AddWithValue("@content", cont_instance.Content);

                                int rows = command.ExecuteNonQuery();
                                if (rows == 0)
                                    return InternalServerError();
                            }
                        }

                    }
                }

                // Return 201 Created + full resource
                return Created(
                    $"/api/somiod/{containerName}/{cont_instance.ResourceName}",
                    cont_instance
                );
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
        public IHttpActionResult Put(string resourceName, [FromBody] container containerPut) // o put apenas muda a app ao qual o container pertence
        {
            
            if (string.IsNullOrWhiteSpace(resourceName) || containerPut == null )
            {
                return BadRequest("check the the resource name and the new container data");
            }
            else
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string query = @"UPDATE container SET [application-resource-name] = @application_resource_name WHERE [resource-name] = @resourceName";

                        

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@application_resource_name", containerPut.ApplicationResourceName);
                            cmd.Parameters.AddWithValue("@resourceName", resourceName);

                            int rows = cmd.ExecuteNonQuery();
                            if (rows == 0)
                                return NotFound();
                        }
                    }
                    // apenas para na resposta nao aparecer estes campos a  null, porque efetivamente nao foram alterados, apenas algo visual
                    containerPut.ResType = "container"; 
                    containerPut.ResourceName = resourceName;

                    return Ok(containerPut);
                }
                catch (SqlException ex)
                {
                    return InternalServerError(ex);
                }
            }
        }

        #endregion

        #region Delete

        [HttpDelete]
        [Route("{containerName}")]
        public IHttpActionResult Delete(string containerName)
        {
            if (string.IsNullOrWhiteSpace(containerName))
                return BadRequest("Missing resource name");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"DELETE FROM container WHERE [resource-name] = @containerName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@containerName", containerName);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0)
                            return NotFound();
                    }
                }

                return Ok($"Container '{containerName}' deleted successfully.");
            }
            catch (SqlException ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion
    }
}
