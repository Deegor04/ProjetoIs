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
    [RoutePrefix("api/somiod")]
    public class contentInstanceController : ApiController
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["ProjetoIs.Properties.Settings.ConnectionString"].ConnectionString;


        //FAZER GETALL AQUI

        //Fazer GET
        [HttpGet]
        [Route("{appName}/{containerName}/{ciName}")]
        public IHttpActionResult Get(string appName, string containerName, string ciName)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"SELECT * FROM [content-instance] WHERE [resource-name] = @name AND [container-resource-name] = @container";
                        

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
        [Route("{appName}/{containerName}/{ciName}")]
        public IHttpActionResult Delete(string appName, string containerName, string ciName)
        {
            if (string.IsNullOrWhiteSpace(ciName))
                return BadRequest("Missing content-instance name");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string deleteQuery = @" DELETE FROM [content-instance] WHERE [resource-name] = @name AND [container-resource-name] = @container";
                       
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
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
