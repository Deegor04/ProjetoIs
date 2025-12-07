using Newtonsoft.Json.Linq;
using ProjetoIs.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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

        #region post content-instance e subscriptions
        [HttpPost]
        [Route("{containerName}")]
        public IHttpActionResult Post(string applicationName, string containerName, [FromBody] JObject body)
        {
            if (body == null)
                return BadRequest("Missing body");

            // 1) Validar resource-name
            string resourceName = (string)body["resource-name"];
            if (string.IsNullOrWhiteSpace(resourceName))
                return BadRequest("Missing field: resource-name");

            // 2) Validar res-type
            string resType = (string)body["res-type"];
            if (string.IsNullOrWhiteSpace(resType))
                return BadRequest("Missing field: res-type");

            resType = resType.ToLower();
            DateTime creation = DateTime.UtcNow;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // -------------------------------------------------------
                    // 0) Validar parent: application + container
                    // -------------------------------------------------------
                    using (var cmd = new SqlCommand(@"
                SELECT COUNT(*) 
                FROM container c 
                JOIN application a 
                  ON a.[resource-name] = c.[application-resource-name]
                WHERE a.[resource-name] = @app 
                  AND c.[resource-name] = @cont", conn))
                    {
                        cmd.Parameters.AddWithValue("@app", applicationName);
                        cmd.Parameters.AddWithValue("@cont", containerName);

                        if ((int)cmd.ExecuteScalar() == 0)
                            return BadRequest("Application or container does not exist.");
                    }

                    // ==================================================================
                    // CASE 1: CONTENT-INSTANCE
                    // ==================================================================
                    if (resType == "content-instance")
                    {
                        // Converter para modelo correto
                        var ci = body.ToObject<content_instance>();

                        // Validar campos obrigatórios
                        if (string.IsNullOrWhiteSpace(ci.ContentType) ||
                            string.IsNullOrWhiteSpace(ci.Content))
                            return BadRequest("Missing content or content-type.");

                        ci.ResType = "content-instance";
                        ci.ContainerResourceName = containerName;
                        ci.CreationDatetime = creation;

                        // Ver duplicado
                        using (var cmdDup = new SqlCommand(
                            "SELECT COUNT(*) FROM [content-instance] WHERE [resource-name] = @n", conn))
                        {
                            cmdDup.Parameters.AddWithValue("@n", ci.ResourceName);
                            bool exists = (int)cmdDup.ExecuteScalar() > 0;

                            string finalName = ci.ResourceName;
                            if (exists)
                                finalName = creation.ToString("yyyyMMdd_HHmmss_fff");

                            // Inserir CI
                            using (var cmdIns = new SqlCommand(@"
                        INSERT INTO [content-instance]
                        ([resource-name],[res-type],[creation-datetime],[container-resource-name],[content-type],[content])
                        VALUES (@n,@t,@dt,@c,@ctype,@content)", conn))
                            {
                                cmdIns.Parameters.AddWithValue("@n", finalName);
                                cmdIns.Parameters.AddWithValue("@t", ci.ResType);
                                cmdIns.Parameters.AddWithValue("@dt", ci.CreationDatetime);
                                cmdIns.Parameters.AddWithValue("@c", ci.ContainerResourceName);
                                cmdIns.Parameters.AddWithValue("@ctype", ci.ContentType);
                                cmdIns.Parameters.AddWithValue("@content", ci.Content);

                                cmdIns.ExecuteNonQuery();

                                ci.ResourceName = finalName;

                                return Created(
                                    $"/api/somiod/{applicationName}/{containerName}/{ci.ResourceName}",
                                    ci);
                            }
                        }
                    }

                    // ==================================================================
                    // CASE 2: SUBSCRIPTION
                    // ==================================================================
                    if (resType == "subscription")
                    {
                        var sub = body.ToObject<subscription>();

                        // Validar evt
                        if (sub.Evt != 1 && sub.Evt != 2)
                            return BadRequest("evt must be 1 or 2");

                        // Validar endpoint
                        if (string.IsNullOrWhiteSpace(sub.Endpoint))
                            return BadRequest("endpoint is required");

                        sub.ResType = "subscription";
                        sub.ContainerResourceName = containerName;
                        sub.CreationDatetime = creation;

                        // Ver duplicado
                        using (var cmdDup = new SqlCommand(@"
                    SELECT COUNT(*) FROM subscription
                    WHERE [resource-name] = @n AND [container-resource-name] = @c", conn))
                        {
                            cmdDup.Parameters.AddWithValue("@n", sub.ResourceName);
                            cmdDup.Parameters.AddWithValue("@c", containerName);

                            if ((int)cmdDup.ExecuteScalar() > 0)
                                return BadRequest("Subscription already exists");
                        }

                        // Inserir subscription
                        using (var cmdIns = new SqlCommand(@"
                    INSERT INTO subscription
                    ([resource-name],[creation-datetime],[container-resource-name],[res-type],[evt],[endpoint])
                    VALUES (@n,@dt,@c,@t,@e,@p)", conn))
                        {
                            cmdIns.Parameters.AddWithValue("@n", sub.ResourceName);
                            cmdIns.Parameters.AddWithValue("@dt", sub.CreationDatetime);
                            cmdIns.Parameters.AddWithValue("@c", sub.ContainerResourceName);
                            cmdIns.Parameters.AddWithValue("@t", sub.ResType);
                            cmdIns.Parameters.AddWithValue("@e", sub.Evt);
                            cmdIns.Parameters.AddWithValue("@p", sub.Endpoint);

                            cmdIns.ExecuteNonQuery();

                            return Created(
                                $"/api/somiod/{applicationName}/{containerName}/subs/{sub.ResourceName}",
                                sub);
                        }
                    }

                    // ==================================================================
                    // CASE 3: OUTRO VALOR INVÁLIDO
                    // ==================================================================
                    return BadRequest("Invalid res-type. Must be 'content-instance' or 'subscription'.");
                }
            }
            catch (Exception ex)
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
