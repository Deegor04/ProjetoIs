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

/*** TO DO
 * 
 * ver o tipo de return que temos de enviar 
 * se e HttpResponseMessage como está no GetApplication
 * ou IHttpActionResult como esta no get e no post
 *
 ***/

namespace ProjetoIs.Controllers
{
    [RoutePrefix("api/somiod")]

    public class applicationController : ApiController
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ProjetoIS.Properties.Settings.ConnectionString"].ConnectionString;


        #region GetAll
        // GET: “somiod-discovery: application” http://<domain:9876>/api/somiod - returns all applications
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            // Only process this if header exists
            IEnumerable<string> headers;
            if (!Request.Headers.TryGetValues("somiod-discovery", out headers))
            {
                return BadRequest("Missing somiod-discovery header");
            }

            string resType = headers.FirstOrDefault();
            if (resType == null) {
                return BadRequest("Invalid somiod-discovery type");
            }
            if(resType.ToLower() == "application") 
            {
                try
                {
                    List<string> paths = new List<string>();

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string query = @"SELECT [resource-name] FROM application";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string name = reader["resource-name"].ToString();
                                paths.Add($"/api/somiod/{name}");
                            }
                        }
                    }

                    return Ok(paths);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
            else
            {
                /*** a minha ideia era aqui chamar os outros gets 
                 * 
                 *  por este url serve tb para outras "classes" por exemplo  “somiod-discovery: content-instance”
                 * 

                 * 
                 *  se o tipo fosse container chamavamos aqui a containerController.get() 
                 *  se o tipo fosse content-instance chamavamos aqui o contentInstanteController.get()
                 *  se fosse subscription chamavamos aqui o subscriptionController.get()
                 *  tudo separdo por if e elses
                 * 
                 ***/

                return InternalServerError();
            }
        }
        #endregion


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

        #region post
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
        #endregion

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
