using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProjetoIs.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ProjetoIs.Controllers
{
    [RoutePrefix("api/somiod")]

    public class applicationController : ApiController
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ProjetoIS.Properties.Settings.ConnectionString"].ConnectionString;

        // GET: api/application
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        #region get
        // Get Application: http://<domain:9876>/api/somiod/app5 - returns app5 data 
        [HttpGet]
        [Route("{resourceName}")]
        public HttpResponseMessage GetApplication(string resourceName)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"SELECT *
                                     FROM application
                                     WHERE [resource-name] = @resourceName";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@resourceName", resourceName);

                        using (var reader = cmd.ExecuteReader())
                        {
                             application app = null;

                            if (reader.Read())
                            {
                                app = new application
                                {
                                    ResourceName = (string)reader["resource-name"],
                                    ResType = (string)reader["res-type"],
                                    CreationDatetime = (DateTime)reader["creation-datetime"]
                                };
                            }

                            if (app != null)
                            {
                                return Request.CreateResponse(HttpStatusCode.OK, app);
                                //var response = Request.CreateResponse(HttpStatusCode.OK, app);
                                //response.Content = new ObjectContent<application>(app, new System.Net.Http.Formatting.XmlMediaTypeFormatter());
                                //return response;
                            }

                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Application not found");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on getting the application: {ex.Message}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error on getting the application");
            }
        }

        #endregion

        // POST: api/application
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/application/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/application/5
        public void Delete(int id)
        {
        }
    }
}
