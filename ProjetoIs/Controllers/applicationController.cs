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
        public IHttpActionResult GetApplication(string resourceName)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"SELECT * FROM application WHERE [resource-name] = @resourceName";

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
                                return Ok(app);
                                //var response = Request.CreateResponse(HttpStatusCode.OK, app);
                                //response.Content = new ObjectContent<application>(app, new System.Net.Http.Formatting.XmlMediaTypeFormatter());
                                //return response;
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
                    string query = @"INSERT INTO application ([resource-name], [res-type], [creation-datetime]) VALUES (@resourceName, @resType, @creationDatetime)";

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

        #region PUT
        [HttpPut]
        [Route("{resourceName}")]
        public IHttpActionResult Put(string resourceName, [FromBody] application app)
        {
            /***
             * da forma que esta implementado nem faz muito sentido enviar qualquer dado no body, o res-type nao muda, a data e enviada por uma funcao
             * mas de qualquer maneira decidi que temos de enviar pelo menos o resouceName
             ***/
            if (string.IsNullOrWhiteSpace(resourceName) || app == null || resourceName != app.ResourceName)
            {
                return BadRequest("check the the resource name and the new application data");
            }
            else
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // doesn't make sense to modify other data
                        string query = @"UPDATE application SET [creation-datetime] = @creationDatetime WHERE [resource-name] = @resourceName";

                        DateTime creation_time = DateTime.UtcNow;

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@creationDatetime", creation_time);
                            cmd.Parameters.AddWithValue("@resourceName", resourceName);

                            int rows = cmd.ExecuteNonQuery();
                            if (rows == 0)
                                return NotFound();
                        }
                        app.CreationDatetime = creation_time;
                    }
                    app.ResType = "application"; // apenas a resposta nao aparecer "res-type": null, porque efetivamente o res-type nao foi alterado, algo apenas visual

                    return Ok(app);
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

                    string query = @"DELETE FROM application WHERE [resource-name] = @resourceName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@resourceName", resourceName);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0)
                            return NotFound();
                    }
                }

                return Ok($"Application '{resourceName}' deleted successfully.");
            }
            catch (SqlException ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion
    }
}
