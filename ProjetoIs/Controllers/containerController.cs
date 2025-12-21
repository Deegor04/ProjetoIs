using Newtonsoft.Json.Linq;
using ProjetoIs.Models;
using ProjetoIs.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace ProjetoIs.Controllers
{
    [RoutePrefix("api/somiod/{applicationName}")]
    public class containerController : ApiController
    {
        private readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ProjetoIs.Properties.Settings.ConnectionString"].ConnectionString;

        private readonly NotificationService _notifier = new NotificationService();

        #region getAll (helper interno p/ discovery global)
        public List<string> Get()
        {
            var paths = new List<string>();

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
                            string containerName = reader["resource-name"].ToString();
                            string appName = reader["application-resource-name"].ToString();
                            paths.Add($"/api/somiod/{appName}/{containerName}");
                        }
                    }
                }

                return paths;
            }
            catch (Exception ex)
            {
                paths.Add(ex.ToString());
                return paths;
            }
        }
        #endregion

        #region get (container OU discovery de content-instances/subscriptions no container)
        [HttpGet]
        [Route("{containerName}")]
        public IHttpActionResult GetContainer(string applicationName, string containerName)
        {
            if (string.IsNullOrWhiteSpace(applicationName) || string.IsNullOrWhiteSpace(containerName))
                return BadRequest("Missing applicationName or containerName");

            // Verificar se somiod-discovery está presente
            if (!Request.Headers.TryGetValues("somiod-discovery", out IEnumerable<string> headers))
            {
                // GET normal → devolver o container
                try
                {
                    using (var conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string query = @"
                            SELECT * 
                            FROM container 
                            WHERE [resource-name] = @containerName
                              AND [application-resource-name] = @applicationName";

                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@containerName", containerName);
                            cmd.Parameters.AddWithValue("@applicationName", applicationName);

                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    var containerGet = new container
                                    {
                                        ResourceName = (string)reader["resource-name"],
                                        ResType = (string)reader["res-type"],
                                        CreationDatetime = (DateTime)reader["creation-datetime"],
                                        ApplicationResourceName = (string)reader["application-resource-name"]
                                    };

                                    return Ok(new
                                    {
                                        containerGet.ResourceName,
                                        containerGet.ResType,
                                        containerGet.CreationDatetime,
                                        containerGet.ApplicationResourceName
                                    });
                                }

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

            // GET com discovery
            string resType = headers.FirstOrDefault()?.Trim();

            if (resType == "content-instance")
            {
                var pathsCi = new List<string>();

                try
                {
                    using (var conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        using (var cmdCheck = new SqlCommand(@"
                            SELECT COUNT(*)
                            FROM container
                            WHERE [resource-name] = @cont
                              AND [application-resource-name] = @app", conn))
                        {
                            cmdCheck.Parameters.AddWithValue("@cont", containerName);
                            cmdCheck.Parameters.AddWithValue("@app", applicationName);

                            if ((int)cmdCheck.ExecuteScalar() == 0)
                                return NotFound();
                        }

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
            if (resType == "subscription")
            {
                var pathsSubs = new List<string>();

                try
                {
                    using (var conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        using (var cmdCheck = new SqlCommand(@"
                            SELECT COUNT(*)
                            FROM container
                            WHERE [resource-name] = @cont
                              AND [application-resource-name] = @app", conn))
                        {
                            cmdCheck.Parameters.AddWithValue("@cont", containerName);
                            cmdCheck.Parameters.AddWithValue("@app", applicationName);

                            if ((int)cmdCheck.ExecuteScalar() == 0)
                                return NotFound();
                        }

                        string query = @"
                            SELECT [resource-name]
                            FROM [subscription]
                            WHERE [container-resource-name] = @container";

                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@container", containerName);

                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string subs = reader["resource-name"].ToString();
                                    pathsSubs.Add($"/api/somiod/{applicationName}/{containerName}/subs/{subs}");
                                }
                            }
                        }
                    }

                    return Ok(pathsSubs);
                }
                catch (SqlException ex)
                {
                    return InternalServerError(ex);
                }
            }

            return BadRequest("Unknown somiod-discovery type");
        }
        #endregion

        #region post content-instance e subscriptions
        [HttpPost]
        [Route("{containerName}")]
        public IHttpActionResult Post(string applicationName, string containerName, [FromBody] JObject body)
        {
            if (string.IsNullOrWhiteSpace(applicationName) || string.IsNullOrWhiteSpace(containerName))
                return BadRequest("Missing applicationName or containerName");

            if (body == null)
                return BadRequest("Missing body");

            // 1) Validar resource-name
            string resourceName = (string)body["resource-name"];
            if (string.IsNullOrWhiteSpace(resourceName))
                return BadRequest("Missing field: resource-name");

            // 2) Validar res-type
            string resType = ((string)body["res-type"])?.Trim();
            if (string.IsNullOrWhiteSpace(resType))
                return BadRequest("Missing field: res-type");

            resType = resType.ToLower();
            DateTime creation = DateTime.UtcNow;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Validar parent: application + container
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

                    // CASE 1: CONTENT-INSTANCE
                    if (resType == "content-instance")
                    {
                        var ci = body.ToObject<content_instance>();

                        if (string.IsNullOrWhiteSpace(ci.ContentType) || string.IsNullOrWhiteSpace(ci.Content))
                            return BadRequest("Missing content or content-type.");

                        ci.ResType = "content-instance";
                        ci.ContainerResourceName = containerName;
                        ci.CreationDatetime = creation;

                        string finalName = ci.ResourceName;

                        using (var cmdDup = new SqlCommand(@"
                            SELECT COUNT(*) 
                            FROM [content-instance] 
                            WHERE [resource-name] = @n
                              AND [container-resource-name] = @c", conn))
                        {
                            cmdDup.Parameters.AddWithValue("@n", finalName);
                            cmdDup.Parameters.AddWithValue("@c", containerName);

                            bool exists = (int)cmdDup.ExecuteScalar() > 0;

                            if (exists)
                            {
                                string timestamp = creation.ToString("yyyyMMdd_HHmmss_fff");
                                finalName = $"{ci.ResourceName}_{timestamp}";
                            }

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

                                _notifier.NotifySubscriptions(applicationName, containerName, finalName, SubscriptionEvent.Creation, conn);

                                ci.ResourceName = finalName;

                                return Created($"/api/somiod/{applicationName}/{containerName}/{ci.ResourceName}", ci);
                            }
                        }
                    }

                    // CASE 2: SUBSCRIPTION
                    if (resType == "subscription")
                    {
                        var sub = body.ToObject<subscription>();

                        if (sub.Evt < 1 || sub.Evt > 3)
                            return BadRequest("evt must be 1 (creation), 2 (deletion) or both");

                        if (string.IsNullOrWhiteSpace(sub.Endpoint))
                            return BadRequest("endpoint is required");

                        sub.ResType = "subscription";
                        sub.ContainerResourceName = containerName;
                        sub.CreationDatetime = creation;

                        using (var cmdDup = new SqlCommand(@"
                            SELECT COUNT(*) 
                            FROM subscription
                            WHERE [resource-name] = @n 
                              AND [container-resource-name] = @c", conn))
                        {
                            cmdDup.Parameters.AddWithValue("@n", sub.ResourceName);
                            cmdDup.Parameters.AddWithValue("@c", containerName);

                            int exists = (int)cmdDup.ExecuteScalar();

                            if (exists > 0)
                            {
                                string timestamp = creation.ToString("yyyyMMdd_HHmmss_fff");
                                sub.ResourceName = $"{sub.ResourceName}_{timestamp}";
                            }
                        }

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

                            return Created($"/api/somiod/{applicationName}/{containerName}/subs/{sub.ResourceName}", sub);
                        }
                    }

                    return BadRequest("Invalid res-type. Must be 'content-instance' or 'subscription'.");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion

        #region PUT (routing consistente + filtra por applicationName)
        [HttpPut]
        [Route("{containerName}")]
        public IHttpActionResult Put(string applicationName, string containerName, [FromBody] container containerPut)
        {
            if (string.IsNullOrWhiteSpace(applicationName) || string.IsNullOrWhiteSpace(containerName) || containerPut == null)
                return BadRequest("Check applicationName, containerName and body.");

            if (string.IsNullOrWhiteSpace(containerPut.ApplicationResourceName))
                return BadRequest("Missing field: application-resource-name");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Atualiza só se o container pertencer à app do URL
                    string query = @"
                        UPDATE container 
                        SET [application-resource-name] = @newApp
                        WHERE [resource-name] = @containerName
                          AND [application-resource-name] = @currentApp";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@newApp", containerPut.ApplicationResourceName);
                        cmd.Parameters.AddWithValue("@containerName", containerName);
                        cmd.Parameters.AddWithValue("@currentApp", applicationName);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0)
                            return NotFound();
                    }
                }

                containerPut.ResType = "container";
                containerPut.ResourceName = containerName;

                return Ok(new
                {
                    containerPut.ResourceName,
                    containerPut.ResType,
                    containerPut.ApplicationResourceName
                });
            }
            catch (SqlException ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion

        #region Delete (routing consistente + filtra por applicationName)
        [HttpDelete]
        [Route("{containerName}")]
        public IHttpActionResult Delete(string applicationName, string containerName)
        {
            if (string.IsNullOrWhiteSpace(applicationName) || string.IsNullOrWhiteSpace(containerName))
                return BadRequest("Missing applicationName or containerName");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        DELETE FROM container 
                        WHERE [resource-name] = @containerName
                          AND [application-resource-name] = @appName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@containerName", containerName);
                        cmd.Parameters.AddWithValue("@appName", applicationName);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0)
                            return NotFound();
                    }
                }

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (SqlException ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion
    }
}
