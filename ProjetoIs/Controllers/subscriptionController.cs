using ProjetoIs.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Configuration;

namespace ProjetoIs.Controllers
{
    [RoutePrefix("api/somiod/")]
    public class subscriptionController : ApiController
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ProjetoIs.Properties.Settings.ConnectionString"].ConnectionString;

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
                WHERE a.[resource-name] = @AppName AND c.[resource-name] = @ContainerName";

            string sqlCheckDuplicate = @"SELECT COUNT(*)
                FROM [container] c JOIN [application] a
                ON a.[resource-name] = c.[application-resource-name]
                WHERE a.[resource-name] = @AppName AND c.[resource-name] = @ContainerName";

            string sqlCommand = @"INSERT INTO [subscription]
                    ([resource-name], [creation-datetime], [container-resource-name], [res-type], [evt], [endpoint])
                    VALUES (@ResourceName, @CreationDatetime, @ContainerResourceName, @ResType, @Evt, @Endpoint)";

            SqlConnection conn = new SqlConnection(connectionString)

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
                        cmdCheckParent.Parameters.AddWithValue("@AppName", appName);
                        cmdCheckParent.Parameters.AddWithValue("@ContainerName", containerName);

                        int containerCount = (int)cmdCheckParent.ExecuteScalar();

                        if (containerCount == 0)
                        {
                            return BadRequest("Application or Container does not exist.");
                        }
                    }

                    // 2) Verificar se já existe subscription com este nome (opcional)
                    using (cmdCheckDuplicate)
                    {
                        cmdCheckDuplicate.Parameters.AddWithValue("@AppName", appName);
                        cmdCheckDuplicate.Parameters.AddWithValue("@ContainerName", containerName);
                        cmdCheckDuplicate.Parameters.AddWithValue("@SubName", value.ResourceName);
                        
                        int subCount = (int)cmdCheckDuplicate.ExecuteScalar();

                        if (subCount > 0)
                        {
                            return BadRequest("Subscription with this name already exists.");
                        }
                    }

                    // 3) Inserir a nova subscription
                    using (cmd)
                    {
                        cmd.Parameters.AddWithValue("@ResourceName", value.ResourceName);
                        cmd.Parameters.AddWithValue("@CreationDatetime", value.CreationDatetime);
                        cmd.Parameters.AddWithValue("@ContainerResourceName", value.ContainerResourceName);
                        cmd.Parameters.AddWithValue("@ResType", value.ResType);
                        cmd.Parameters.AddWithValue("@Evt", value.Evt);
                        cmd.Parameters.AddWithValue("@Endpoint", value.Endpoint);

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



        /*
        [HttpGet]
        [Route("{applicationName}/{containerName}/subs/{subName}")]
        public IHttpActionResult GetSubscription(string appName, string containerName, string subName)
        {
            var subscription = GetSubscriptionFromDb(appName, containerName, subName);
            if (subscription == null)
            {
                return NotFound();
            }

            return Ok(subscription);
        }

        public subscription GetSubscriptionFromDb(string appName, string containerName, string subName)
        {
            const string connDb = @"SELECT s.[resource-name], s.[creation-datetime], s.[container-resource-name],
                s.[res-type], s.[evt], s.[endpoint]
                FROM [subscription] s JOIN [container] c ON c.[resource-name] = s.[container-resource-name]
                JOIN [application] a ON a.[resource-name] = c.[application-resource-name]
                WHERE a.[resource-name] = @AppName
                AND c.[resource-name] = @ContainerName
                AND s.[resource-name] = @SubName";

            var conn = new SqlConnection(connectionString);
            var cmd = new SqlCommand(connDb, conn);
            var reader = cmd.ExecuteReader();

            using (cmd)
            {
                cmd.Parameters.AddWithValue("@AppName", appName);
                cmd.Parameters.AddWithValue("@ContainerName", containerName);
                cmd.Parameters.AddWithValue("@SubName", subName);

                conn.Open();

                using (reader)
                {
                    
                }
            }
            
        }
        */

    }
}
