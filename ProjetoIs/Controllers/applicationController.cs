using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProjetoIs.Models;
using static System.Net.Mime.MediaTypeNames;
/*************
 * 
 * Install-Package Newtonsoft.Json
 * 
 *************/

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
            // ns se meter uma condicao para se o resource name for vazio faz sentido, pq isso para um getAll;
            // ver condicoes do get all
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

        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] application app)
        {
            if (app == null || string.IsNullOrWhiteSpace(app.ResourceName))
            {

                return BadRequest("Missing required field: resource-name");
            }

            app.ResType = "application"; // aqui
            app.CreationDatetime = DateTime.UtcNow; // aqui: perguntar ao professor se o o utilizador e susposto enviar tudo

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO application
                                 ([resource-name], [res-type], [creation-datetime])
                                 VALUES (@resourceName, @resType, @creationDatetime)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@resourceName", app.ResourceName);
                        cmd.Parameters.AddWithValue("@resType", app.ResType);
                        cmd.Parameters.AddWithValue("@creationDatetime", app.CreationDatetime);
                        cmd.Connection = conn;
                        int rows = cmd.ExecuteNonQuery();
                        if (rows <= 0)
                            return InternalServerError();
                    }
                }

                // return 201 Created + full resource
                return Created($"/api/somiod/{app.ResourceName}",app);
            }
            catch (SqlException e)
            {
                return InternalServerError(e);
            }
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
