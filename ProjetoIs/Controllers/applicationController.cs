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

/*************
*
* TO DO - Organizar o codigo, tenho muito codigo repedito entre classes e metodos ( entre alguns if e elses)
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
            else
            {
                if (resType.ToLower() == "container")
                { 
                    var controller = new containerController();
                    List<string> pathsContainer = controller.Get();
                    return Ok(pathsContainer);
                }

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
            IEnumerable<string> headers;
            if (!Request.Headers.TryGetValues("somiod-discovery", out headers))
            {
                try
                {
                    using (var conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string query = @"SELECT * FROM application WHERE [resource-name] = @resourceName";

                        // the left join secures that an application without a container will be returned - se quissesemos dar return as app com o nome dos containers "filhos"
                        //string query = @"SELECT a.*, c.[resource-name] as container_resource_name FROM [dbo].[application] a LEFT JOIN [dbo].[container] c ON a.[resource-name] = c.[application-resource-name] WHERE a.[resource-name] = @resourceName";

                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@resourceName", resourceName);

                            using (var reader = cmd.ExecuteReader())
                            {
                                application app = null;
                                //var containers = new List<container>();

                                if (reader.Read())
                                {
                                    app = new application
                                    {
                                        ResourceName = (string)reader["resource-name"],
                                        ResType = (string)reader["res-type"],
                                        CreationDatetime = (DateTime)reader["creation-datetime"],
                                        //Containers = new List<Container>()
                                    };
                                    /*if (!reader.IsDBNull(reader.GetOrdinal("container_resource_name")))
                                    {
                                        var container = new container
                                        {
                                            ResourceName = (string)reader["container_resource_name"],
                                        };
                                        containers.Add(container);
                                    }*/
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

            string resType = headers.FirstOrDefault();
            if (resType == "container")
            {
                List<string> pathsApplicacion = new List<string>();
                try
                {

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string query = @"SELECT [resource-name],[application-resource-name] FROM container WHERE [application-resource-name] = @application_resource_name";


                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@application_resource_name", resourceName);
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
            else // TO DO - add if (resType == "content-instance")
            {
                return InternalServerError();
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
                            string uniqueName = app.CreationDatetime.ToString("yyyyMMdd_HHmmss_fff");

                            string query = @"INSERT INTO application ([resource-name], [res-type], [creation-datetime]) VALUES (@resourceName, @resType, @creationDatetime)";

                            using (SqlCommand command = new SqlCommand(query, conn))
                            {
                                command.Parameters.AddWithValue("@resourceName", uniqueName); // cada data e unica, por isso o nome vai ser sempre unico
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
                            string uniqueName = container.CreationDatetime.ToString("yyyyMMdd_HHmmss_fff");

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
        [Route("{resourceName}")]
        public IHttpActionResult Put(string resourceName, [FromBody] application app)
        {
            /***
             * da forma que esta implementado nem faz muito sentido enviar qualquer dado no body, o res-type nao muda, a data e enviada por uma funcao
             * mas de qualquer maneira decidi que temos de enviar pelo menos o resouceName, embora a unica coisa alterada seja a creation-date
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
