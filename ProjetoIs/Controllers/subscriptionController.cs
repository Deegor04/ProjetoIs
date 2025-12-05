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
    [RoutePrefix("api/somiod")]
    public class subscriptionController : ApiController
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ProjetoIs.Properties.Settings.ConnectionString"].ConnectionString;

        [HttpGet]
        [Route("{appName}/{containerName}/subs")]
        public IHttpActionResult GetAllSubs()
        {
            IEnumerable<string> headers;
            if (!Request.Headers.TryGetValues("somiod-discovery", out headers)) // verificar se o header somiod-discovery esta presente
            {
                return BadRequest("Missing somiod-discovery header");
            }

            string resType = headers.FirstOrDefault();
            if (resType == null)
            {
                return BadRequest("Invalid somiod-discovery type");
            }

            resType = resType.ToLower();

            if (resType == "subscription")
            {
                try
                {
                    List<string> pathsSubs = new List<string>();

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string query = @"
                            SELECT  s.[resource-name]           AS sub_name,
                                    c.[resource-name]           AS cont_name,
                                    a.[resource-name]           AS app_name
                            FROM [subscription] s
                            JOIN [container]   c ON c.[resource-name] = s.[container-resource-name]
                            JOIN [application] a ON a.[resource-name] = c.[application-resource-name];";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string sub = reader["sub_name"].ToString();
                                string cont = reader["cont_name"].ToString();
                                string app = reader["app_name"].ToString();

                                // caminho SOMIOD para cada subscrição
                                pathsSubs.Add($"/api/somiod-subscription/{app}/{cont}/subs/{sub}");
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
            else if (resType == "container")
            {
                var controller = new containerController();
                List<string> pathsContainer = controller.Get();
                return Ok(pathsContainer);
            }
            else if (resType == "content-instance")
            {
                var controller = new content_instanceController();
                List<string> pathsCi = controller.Get();
                return Ok(pathsCi);
            }

            return BadRequest("Unknown somiod-discovery type");

        }

        [HttpGet]
        [Route("{appName}/{containerName}/subs/{subName}")]
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

        [HttpPost]
        [Route("{appName}/{containerName}/subs")]
        public IHttpActionResult Post(string appName, string containerName, [FromBody] subscription value)
        {
            if (value == null)
                return BadRequest("There is no body at the moment.");


            if (string.IsNullOrWhiteSpace(value.ResourceName))
                value.ResourceName = "sub-" + Guid.NewGuid().ToString();


            if (value.Evt != 1 && value.Evt != 2)
                return BadRequest("evt inválido (usa 1 ou 2).");

            if (string.IsNullOrWhiteSpace(value.Endpoint))
                return BadRequest("endpoint é obrigatório.");

            value.ResType = "subscription";
            value.ContainerResourceName = containerName;
            value.CreationDatetime = DateTime.UtcNow;

            string sqlCheckParent = @"SELECT COUNT(*)
                FROM [container] c JOIN [application] a
                ON a.[resource-name] = c.[application-resource-name]
                WHERE a.[resource-name] = @applicationName AND c.[resource-name] = @containerName";

            string sqlCheckDuplicate = @"SELECT COUNT(*)
                FROM [subscription]
                WHERE [resource-name] = @subName
                AND [container-resource-name] = @containerName";


            string sqlCommand = @"INSERT INTO [subscription]
                    ([resource-name], [creation-datetime], [container-resource-name], [res-type], [evt], [endpoint])
                    VALUES (@resourceName, @creationDatetime, @containerResourceName, @resType, @evt, @endpoint)";

            SqlConnection conn = new SqlConnection(connectionString);

            var cmd = new SqlCommand(sqlCommand, conn);
            var cmdCheckParent = new SqlCommand(sqlCheckParent, conn);
            var cmdCheckDuplicate = new SqlCommand(sqlCheckDuplicate, conn);

            try
            {
                using (conn)
                {

                    conn.Open();

                    // 1) Verificar se app + container existem
                    using (cmdCheckParent)
                    {
                        cmdCheckParent.Parameters.AddWithValue("@applicationName", appName);
                        cmdCheckParent.Parameters.AddWithValue("@containerName", containerName);

                        int containerCount = (int)cmdCheckParent.ExecuteScalar();

                        if (containerCount == 0)
                        {
                            return BadRequest("Application or Container does not exist.");
                        }
                    }

                    // 2) Verificar se já existe subscription com este nome
                    using (cmdCheckDuplicate)
                    {
                        cmdCheckDuplicate.Parameters.AddWithValue("@subName", value.ResourceName);
                        cmdCheckDuplicate.Parameters.AddWithValue("@containerName", containerName);

                        int subCount = (int)cmdCheckDuplicate.ExecuteScalar();

                        if (subCount > 0)
                        {
                            return BadRequest("Subscription with this name already exists.");
                        }
                    }

                    // 3) Inserir a nova subscription
                    using (cmd)
                    {
                        cmd.Parameters.AddWithValue("@resourceName", value.ResourceName);
                        cmd.Parameters.AddWithValue("@creationDatetime", value.CreationDatetime);
                        cmd.Parameters.AddWithValue("@containerResourceName", value.ContainerResourceName);
                        cmd.Parameters.AddWithValue("@resType", value.ResType);
                        cmd.Parameters.AddWithValue("@evt", value.Evt);
                        cmd.Parameters.AddWithValue("@endpoint", value.Endpoint);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(value);
                        }
                        else
                        {
                            return InternalServerError(new Exception("Failed to create subscription."));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("{appName}/{containerName}/subs/{subName}")]
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
