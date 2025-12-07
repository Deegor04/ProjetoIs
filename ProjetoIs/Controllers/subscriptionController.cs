using ProjetoIs.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;

namespace ProjetoIs.Controllers
{
    [RoutePrefix("api/somiod/{applicationName}/{containerName}/subs")]
    public class subscriptionController : ApiController
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ProjetoIs.Properties.Settings.ConnectionString"].ConnectionString;

        [HttpGet]
        [Route("")]
        public List<string> GetAllSubs()
        {
            var pathsSubscriptions = new List<string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                SELECT  s.[resource-name]            AS sub_name,
                        s.[container-resource-name] AS cont_name,
                        c.[application-resource-name] AS app_name
                FROM [subscription] s
                JOIN container c
                    ON s.[container-resource-name] = c.[resource-name];";

                    using (var cmd = new SqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string sub = reader["sub_name"].ToString();
                            string cont = reader["cont_name"].ToString();
                            string app = reader["app_name"].ToString();

                            // caminho SOMIOD padrão para subscrições
                            pathsSubscriptions.Add($"/api/somiod/{app}/{cont}/subs/{sub}");
                        }
                    }
                }

                return pathsSubscriptions;
            }
            catch (Exception ex)
            {
                pathsSubscriptions.Add(ex.ToString());
                return pathsSubscriptions;
            }
        }


        [HttpGet]
        [Route("{subName}")]
        public IHttpActionResult GetSubscription(string appName, string containerName, string subName)
        {
            IEnumerable<string> headers;
            if (!Request.Headers.TryGetValues("somiod-discovery", out headers))
            {
                //  Header não presente - devolver objeto da subscrição
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
                            FROM [subscription] s
                            JOIN [container] c
                              ON c.[resource-name] = s.[container-resource-name]
                            JOIN [application] a
                              ON a.[resource-name] = c.[application-resource-name]
                            WHERE a.[resource-name] = @appName
                              AND c.[resource-name] = @containerName
                              AND s.[resource-name] = @subName;";

                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@appName", appName);
                            cmd.Parameters.AddWithValue("@containerName", containerName);
                            cmd.Parameters.AddWithValue("@subName", subName);

                            using (var reader = cmd.ExecuteReader())
                            {
                                subscription sub = null;

                                if (reader.Read())
                                {
                                    sub = new subscription
                                    {
                                        ResourceName = (string)reader["resource-name"],
                                        CreationDatetime = (DateTime)reader["creation-datetime"],
                                        ContainerResourceName = (string)reader["container-resource-name"],
                                        ResType = (string)reader["res-type"],
                                        Evt = (int)reader["evt"],
                                        Endpoint = (string)reader["endpoint"]
                                    };
                                }

                                if (sub != null)
                                    return Ok(sub);

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
            else
            {
                // Header presente - devolver paths das subscrições desta aplicação
                string resType = headers.FirstOrDefault();
                if (resType == null || resType.ToLower() != "subscription")
                    return BadRequest("Invalid somiod-discovery type");

                try
                {
                    List<string> pathsSubs = new List<string>();

                    using (var conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string query = @"
                            SELECT  s.[resource-name]           AS sub_name,
                                    c.[resource-name]           AS cont_name,
                                    a.[resource-name]           AS app_name
                            FROM [subscription] s
                            JOIN [container] c
                              ON c.[resource-name] = s.[container-resource-name]
                            JOIN [application] a
                              ON a.[resource-name] = c.[application-resource-name]
                            WHERE a.[resource-name] = @appName;";

                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@appName", appName);

                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string sub = reader["sub_name"].ToString();
                                    string cont = reader["cont_name"].ToString();
                                    string app = reader["app_name"].ToString();

                                    pathsSubs.Add($"/api/somiod-subscription/{app}/{cont}/subs/{sub}");
                                }
                            }
                        }
                    }

                    return Ok(pathsSubs);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }

        [HttpDelete]
        [Route("{subName}")]
        public IHttpActionResult DeleteSubscription(string appName, string containerName, string subName)
        {
            var conn = new SqlConnection(connectionString);

            string deleteQuery = @"
                DELETE s
                FROM [subscription] s
                JOIN [container] c ON c.[resource-name] = s.[container-resource-name]
                JOIN [application] a ON a.[resource-name] = c.[application-resource-name]
                WHERE a.[resource-name] = @appName
                  AND c.[resource-name] = @containerName
                  AND s.[resource-name] = @subName";

            var cmd = new SqlCommand(deleteQuery, conn);

            cmd.Parameters.AddWithValue("@appName", appName);
            cmd.Parameters.AddWithValue("@containerName", containerName);
            cmd.Parameters.AddWithValue("@subName", subName);

            try
            {
                using (conn)
                {
                    conn.Open();

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        return NotFound();
                    }
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
