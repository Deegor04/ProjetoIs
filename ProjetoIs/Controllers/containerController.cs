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
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ProjetoIS.Properties.Settings.ConnectionString"].ConnectionString;

        // GET: api/container
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        #region get
        // Get Application: http://<domain:9876>/api/somiod/app5 - returns app5 data 
        [HttpGet]
        [Route("{resourceName}")]
        public IHttpActionResult GetApplication(string resourceName)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"SELECT * FROM container WHERE [resource-name] = @resourceName";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@resourceName", resourceName);

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
                                return Ok(new { containerGet });
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
        #endregion

        #region post content-instance

        #endregion


        #region PUT
        [HttpPut]
        [Route("{resourceName}")]
        public IHttpActionResult Put(string resourceName, [FromBody] container containerPut)
        {
            
            if (string.IsNullOrWhiteSpace(resourceName) || containerPut == null /*|| resourceName != containerPut.ResourceName */)
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
        [Route("{resourceName}")]
        public IHttpActionResult Delete(string resourceName)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
                return BadRequest("Missing resource name");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"DELETE FROM container WHERE [resource-name] = @resourceName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@resourceName", resourceName);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0)
                            return NotFound();
                    }
                }

                return Ok($"Container '{resourceName}' deleted successfully.");
            }
            catch (SqlException ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion
    }
}
