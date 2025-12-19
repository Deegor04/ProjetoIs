using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Net;
using ProjetoIs.Models;
using ProjetoIs.Services;

namespace ProjetoIs.Controllers
{
    [RoutePrefix("api/somiod/{applicationName}/{containerName}")]
    public class content_instanceController : ApiController
    {
        string connectionString = ConfigurationManager.ConnectionStrings["ProjetoIs.Properties.Settings.ConnectionString"].ConnectionString;
        private readonly NotificationService _notifier = new NotificationService();

        public List<string> Get()
        {
            var pathsContent_instance = new List<string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT  ci.[resource-name]            AS ci_name,
                                ci.[container-resource-name] AS cont_name,
                                c.[application-resource-name] AS app_name
                        FROM [content-instance] ci
                        JOIN container c
                          ON ci.[container-resource-name] = c.[resource-name];";

                    using (var cmd = new SqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string ci = reader["ci_name"].ToString();
                            string cont = reader["cont_name"].ToString();
                            string app = reader["app_name"].ToString();

                            pathsContent_instance.Add($"/api/somiod/{app}/{cont}/{ci}");
                        }
                    }
                }

                return pathsContent_instance;
            }
            catch (Exception ex)
            {
                pathsContent_instance.Add(ex.ToString());
                return pathsContent_instance;
            }
        }


        [HttpGet]
        [Route("{ciName}")]
        public IHttpActionResult Get(string applicationName, string containerName, string ciName)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT * 
                        FROM [content-instance] 
                        WHERE [resource-name] = @name 
                          AND [container-resource-name] = @container";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", ciName);
                        cmd.Parameters.AddWithValue("@container", containerName);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                                return NotFound();

                            var ci = new content_instance
                            {
                                ResourceName = (string)reader["resource-name"],
                                ResType = (string)reader["res-type"],
                                CreationDatetime = (DateTime)reader["creation-datetime"],
                                ContainerResourceName = (string)reader["container-resource-name"],
                                ContentType = reader["content-type"] as string,
                                Content = reader["content"] as string
                            };

                            return Ok(ci);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("{ciName}")]
        public IHttpActionResult Delete(string applicationName, string containerName, string ciName)
        {
            if (string.IsNullOrWhiteSpace(ciName))
                return BadRequest("Missing content-instance name");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // 1️ Garantir que o ci pertence ao container e à aplicação
                    string deleteQuery = @"
                DELETE ci
                FROM [content-instance] ci
                JOIN container c
                  ON ci.[container-resource-name] = c.[resource-name]
                WHERE ci.[resource-name] = @ciName
                  AND ci.[container-resource-name] = @containerName
                  AND c.[application-resource-name] = @applicationName";

                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ciName", ciName);
                        cmd.Parameters.AddWithValue("@containerName", containerName);
                        cmd.Parameters.AddWithValue("@applicationName", applicationName);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0)
                            return NotFound();
                    }

                    // 2️ Notificar subscriptions (evt = 2)
                    _notifier.NotifySubscriptions(
                        applicationName,
                        containerName,
                        ciName,
                        SubscriptionEvent.Deletion,
                        conn
                    );
                }

                // 3️ Confirmar Delete
                return Ok($"Content Instance '{ciName}' deleted successfully.");
            }
            catch (SqlException ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}