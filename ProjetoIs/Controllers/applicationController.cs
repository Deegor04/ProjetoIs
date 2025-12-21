using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
        string connectionString = ConfigurationManager
    .ConnectionStrings["ProjetoIs.Properties.Settings.ConnectionString"]
    .ConnectionString;


        #region GetAll
        // GET: “somiod-discovery: application” http://<domain:9876>/api/somiod - returns all applications
        [HttpGet]
        [Route("")]

        public IHttpActionResult Get()
        {
            
            IEnumerable<string> headers;
            if (!Request.Headers.TryGetValues("somiod-discovery", out headers)) // verificar se o header somiod-discovery esta presente
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
                    List<string> pathsApplicacion = new List<string>();

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
                                pathsApplicacion.Add($"/api/somiod/{name}");
                            }
                        }
                    }

                    return Ok(pathsApplicacion);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
            else if (resType.ToLower() == "container")
            {
                
                    var controller = new containerController();
                    List<string> pathsContainer = controller.Get();
                    return Ok(pathsContainer);
                
                
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
            }
            else if (resType.ToLower() == "content-instance")
            {

                var controller = new content_instanceController();
                List<string> pathsCi = controller.Get();
                return Ok(pathsCi);
            }
            else if (resType.ToLower() == "subscription")
            {
                var controller = new subscriptionController();
                List<string> pathsSubs = controller.Get();
                return Ok(pathsSubs);
            }
            return InternalServerError();
        }
        
        #endregion

        #region get
        // Get Application: http://<domain:9876>/api/somiod/app5 - returns app5 data 
        [HttpGet]
        [Route("{applicationName}")]
        public IHttpActionResult GetApplication(string applicationName)
        {
            IEnumerable<string> headers;
            if (!Request.Headers.TryGetValues("somiod-discovery", out headers)) // verificar se o header somiod-discovery esta presente
            {
                // se header somiod-discovery nao esta presente -> vamos dar return de uma app; 
                try
                {
                    using (var conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string query = @"SELECT * FROM application WHERE [resource-name] = @applicationName";

                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@applicationName", applicationName);

                            using (var reader = cmd.ExecuteReader())
                            {
                                application app = null;

                                if (reader.Read())
                                {
                                    app = new application
                                    {
                                        ResourceName = (string)reader["resource-name"],
                                        ResType = (string)reader["res-type"],
                                        CreationDatetime = (DateTime)reader["creation-datetime"],
                                    };
                                }

                                if (app != null)
                                {
                                    return Ok(new { app.ResourceName, app.ResType, app.CreationDatetime });
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
            else
            {
                // se o somiod-discovery esta presente
                string resType = headers.FirstOrDefault();
                if (resType == "container")
                {
                    List<string> pathsApplicacion = new List<string>();
                    try
                    {

                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();

                            string query = @"SELECT [resource-name],[application-resource-name] FROM container WHERE [application-resource-name] = @applicationName";


                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@applicationName", applicationName);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string container_name = reader["resource-name"].ToString();
                                        string app_name = reader["application-resource-name"].ToString();
                                        pathsApplicacion.Add($"/api/somiod/{app_name}/{container_name}");
                                    }
                                }
                            }
                        }

                        return Ok(pathsApplicacion);
                    }
                    catch (Exception ex)
                    {
                        return InternalServerError(ex);
                    }
                }
                else if (resType == "content-instance")
                {
                    var pathsCi = new List<string>();

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string query = @"
                            SELECT  ci.[resource-name]            AS ci_name,
                                    ci.[container-resource-name] AS cont_name
                            FROM [content-instance] ci
                            JOIN container c
                              ON ci.[container-resource-name] = c.[resource-name]
                            WHERE c.[application-resource-name] = @applicationName;";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@applicationName", applicationName);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string content_instanceName = reader["ci_name"].ToString();
                                    string containerName = reader["cont_name"].ToString();

                                    pathsCi.Add($"/api/somiod/{applicationName}/{containerName}/{content_instanceName}");
                                }
                            }
                        }
                    }

                    return Ok(pathsCi);
                }
                else if (resType == "subscription")
                {
                    var pathsSub = new List<string>();

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string query = @"
                            SELECT  sub.[resource-name]            AS subscription_name,
                                    sub.[container-resource-name] AS cont_name
                            FROM [subscription] sub
                            JOIN container c
                              ON sub.[container-resource-name] = c.[resource-name]
                            WHERE c.[application-resource-name] = @applicationName;";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@applicationName", applicationName);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string subscription_name = reader["subscription_name"].ToString();
                                    string containerName = reader["cont_name"].ToString();

                                    pathsSub.Add($"/api/somiod/{applicationName}/{containerName}/subs/{subscription_name}");
                                }
                            }
                        }
                    }

                    return Ok(pathsSub);
                }

                else
                {
                    return BadRequest("Unknown somiod-discovery type");
                }
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


            app.ResType = "application"; 
            app.CreationDatetime = DateTime.UtcNow;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // 1) Verificar se a aplicação existe
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM application WHERE [resource-name] = @app",
                        conn))
                    {
                        cmd.Parameters.AddWithValue("@app", app.ResourceName);
                        int exists = (int)cmd.ExecuteScalar();

                        if (exists == 0) // aplicacao nao existe
                        {
                            string query = @"INSERT INTO application ([resource-name], [res-type], [creation-datetime]) VALUES (@resourceName, @resType, @creationDatetime)";

                            using (SqlCommand command = new SqlCommand(query, conn))
                            {
                                command.Parameters.AddWithValue("@resourceName", app.ResourceName);
                                command.Parameters.AddWithValue("@resType", app.ResType);
                                command.Parameters.AddWithValue("@creationDatetime", app.CreationDatetime);
                                command.Connection = conn;
                                int rows = command.ExecuteNonQuery();
                                if (rows <= 0)
                                    return InternalServerError();
                            }
                        }
                        else // aplicacao ja existe -> temos de criar uma com um nome unico
                        {
                            string timestamp = app.CreationDatetime.ToString("yyyyMMdd_HHmmss_fff");
                            string uniqueName = $"{app.ResourceName}_{timestamp}";

                            string query = @"INSERT INTO application ([resource-name], [res-type], [creation-datetime]) VALUES (@resourceName, @resType, @creationDatetime)";

                            using (SqlCommand command = new SqlCommand(query, conn))
                            {
                                command.Parameters.AddWithValue("@resourceName", uniqueName);
                                command.Parameters.AddWithValue("@resType", app.ResType);
                                command.Parameters.AddWithValue("@creationDatetime", app.CreationDatetime);
                                app.ResourceName = uniqueName;
                                command.Connection = conn;
                                int rows = command.ExecuteNonQuery();
                                if (rows <= 0)
                                    return InternalServerError();

                            }
                        }
                            
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

        #region post container
        [HttpPost]
        [Route("{applicationName}")]
        public IHttpActionResult Post(string applicationName, [FromBody] container container)
        {
            if (string.IsNullOrWhiteSpace(applicationName) ||
                container == null ||
                string.IsNullOrWhiteSpace(container.ResourceName))
            {
                return BadRequest("Missing required field: resource-name or invalid container data");
            }

            container.ResType = "container";
            container.CreationDatetime = DateTime.UtcNow;
            container.ApplicationResourceName = applicationName;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // 1) Verificar se a aplicação existe
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM application WHERE [resource-name] = @app",
                        conn))
                    {
                        cmd.Parameters.AddWithValue("@app", applicationName);
                        int exists = (int)cmd.ExecuteScalar();

                        if (exists == 0)
                            return NotFound(); // aplicação não existe
                    }

                    // 2) Verificar se container já existe (nome é único)
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM container WHERE [resource-name] = @c",
                        conn))
                    {
                        cmd.Parameters.AddWithValue("@c", container.ResourceName);
                        int exists = (int)cmd.ExecuteScalar();

                        if (exists > 0)
                        {
                            string timestamp = container.CreationDatetime.ToString("yyyyMMdd_HHmmss_fff");
                            string uniqueName = $"{container.ResourceName}_{timestamp}";

                            string insertQuery = @"INSERT INTO container ([resource-name], [res-type], [creation-datetime], [application-resource-name]) VALUES (@resourceName, @resType, @creationDatetime, @applicationName)";

                            using (SqlCommand command = new SqlCommand(insertQuery, conn))
                            {
                                command.Parameters.AddWithValue("@resourceName", uniqueName);
                                command.Parameters.AddWithValue("@resType", container.ResType);
                                command.Parameters.AddWithValue("@creationDatetime", container.CreationDatetime);
                                command.Parameters.AddWithValue("@applicationName", container.ApplicationResourceName);
                                container.ResourceName = uniqueName;

                                int rows = command.ExecuteNonQuery();
                                if (rows == 0)
                                    return InternalServerError();
                            }
                        }
                        else
                        {
                            // 3) Inserir container
                            string insertQuery = @"INSERT INTO container ([resource-name], [res-type], [creation-datetime], [application-resource-name]) VALUES (@resourceName, @resType, @creationDatetime, @applicationName)";

                            using (SqlCommand command = new SqlCommand(insertQuery, conn))
                            {
                                command.Parameters.AddWithValue("@resourceName", container.ResourceName);
                                command.Parameters.AddWithValue("@resType", container.ResType);
                                command.Parameters.AddWithValue("@creationDatetime", container.CreationDatetime);
                                command.Parameters.AddWithValue("@applicationName", container.ApplicationResourceName);

                                int rows = command.ExecuteNonQuery();
                                if (rows == 0)
                                    return InternalServerError();
                            }
                        }
                            
                    }
                }

                // Return 201 Created + full resource
                return Created(
                    $"/api/somiod/{applicationName}/{container.ResourceName}",
                    container
                );
            }
            catch (SqlException ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion

        #region PUT
        [HttpPut]
        [Route("{applicationName}")]
        public IHttpActionResult Put(string applicationName, [FromBody] application app)
        {
            /***
             * da forma que esta implementado nem faz muito sentido enviar qualquer dado no body, o res-type nao muda, a data e enviada por uma funcao
             * mas de qualquer maneira decidi que temos de enviar pelo menos o resouceName, embora a unica coisa alterada seja a creation-date
             ***/
            if (string.IsNullOrWhiteSpace(applicationName) || app == null || applicationName != app.ResourceName)
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
                            cmd.Parameters.AddWithValue("@resourceName", applicationName);

                            int rows = cmd.ExecuteNonQuery();
                            if (rows == 0)
                                return NotFound();
                        }
                        app.CreationDatetime = creation_time;
                    }
                    app.ResType = "application"; // apenas para na resposta nao aparecer "res-type": null, porque efetivamente o res-type nao foi alterado, algo apenas visual

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
        [Route("{applicationName}")]
        public IHttpActionResult Delete(string applicationName)
        {
            if (string.IsNullOrWhiteSpace(applicationName))
                return BadRequest("Missing application Name");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"DELETE FROM application WHERE [resource-name] = @applicationName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@applicationName", applicationName);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0)
                            return NotFound();
                    }
                }

                return Ok($"Application '{applicationName}' deleted successfully.");
            }
            catch (SqlException ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion
    }
}
