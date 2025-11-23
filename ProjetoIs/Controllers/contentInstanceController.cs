using ProjetoIs.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProjetoIs.Controllers
{
    // /api/somiod/{applicationName}/{containerName}/...
    [RoutePrefix("api/somiod/{applicationName}/{containerName}")]
    public class contentInstanceController : ApiController
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["ProjetoIs.Properties.Settings.ConnectionString"]
                              .ConnectionString;

        
        
        public List<string> Get()
        {
            var pathsApplicacion = new List<string>();

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

                            pathsApplicacion.Add($"/api/somiod/{app}/{cont}/{ci}");
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

                    string deleteQuery = @"
                        DELETE FROM [content-instance] 
                        WHERE [resource-name] = @name 
                          AND [container-resource-name] = @container";

                    using (var cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", ciName);
                        cmd.Parameters.AddWithValue("@container", containerName);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0)
                            return NotFound();
                    }
                }

                return Ok($"Content Instance '{ciName}' deleted successfully.");
            }
            catch (SqlException ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
