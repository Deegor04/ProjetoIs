using ProjetoIs.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;

namespace ProjetoIs.Controllers
{
    [RoutePrefix("api/somiod/{applicationName}/{containerName}/subs")]
    public class subscriptionController : ApiController
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["ProjetoIs.Properties.Settings.ConnectionString"].ConnectionString;

        public List<string> Get()
        {
            var list = new List<string>();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT s.[resource-name] AS sub,
                           c.[resource-name] AS cont,
                           a.[resource-name] AS app
                    FROM subscription s
                    JOIN container c ON c.[resource-name] = s.[container-resource-name]
                    JOIN application a ON a.[resource-name] = c.[application-resource-name]";

                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(
                            $"/api/somiod/{reader["app"]}/{reader["cont"]}/subs/{reader["sub"]}"
                        );
                    }
                }
            }

            return list;
        }

        // =====================================================================
        // 2) GET SPECIFIC SUBSCRIPTION BY NAME
        // =====================================================================
        [HttpGet]
        [Route("{subName}")]
        public IHttpActionResult GetSubscription(string applicationName, string containerName, string subName)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT s.[resource-name],
                               s.[creation-datetime],
                               s.[container-resource-name],
                               s.[res-type],
                               s.[evt],
                               s.[endpoint]
                        FROM subscription s
                        JOIN container c 
                            ON c.[resource-name] = s.[container-resource-name]
                        JOIN application a 
                            ON a.[resource-name] = c.[application-resource-name]
                        WHERE a.[resource-name] = @app
                          AND c.[resource-name] = @cont
                          AND s.[resource-name] = @sub";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@app", applicationName);
                        cmd.Parameters.AddWithValue("@cont", containerName);
                        cmd.Parameters.AddWithValue("@sub", subName);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                                return NotFound(); // não existe subscrição

                            var sub = new subscription
                            {
                                ResourceName = reader["resource-name"].ToString(),
                                CreationDatetime = (DateTime)reader["creation-datetime"],
                                ContainerResourceName = reader["container-resource-name"].ToString(),
                                ResType = reader["res-type"].ToString(),
                                Evt = (int)reader["evt"],
                                Endpoint = reader["endpoint"].ToString()
                            };

                            return Ok(sub);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }




        // =====================================================================
        // 3) DELETE SUBSCRIPTION
        // =====================================================================
        [HttpDelete]
        [Route("{subName}")]
        public IHttpActionResult DeleteSubscription(
            string applicationName,
            string containerName,
            string subName)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        DELETE s
                        FROM subscription s
                        JOIN container c ON c.[resource-name] = s.[container-resource-name]
                        JOIN application a ON a.[resource-name] = c.[application-resource-name]
                        WHERE a.[resource-name] = @app
                          AND c.[resource-name] = @cont
                          AND s.[resource-name] = @sub";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@app", applicationName);
                        cmd.Parameters.AddWithValue("@cont", containerName);
                        cmd.Parameters.AddWithValue("@sub", subName);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows == 0)
                            return NotFound();

                        return Ok();
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        
    }
}
