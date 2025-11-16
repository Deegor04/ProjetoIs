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

        // GET: api/container/5
        public string Get(int id)
        {
            return "value";
        }
        /*
        #region post
        [HttpPost]
        [Route("")]
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
                            return Conflict(); // nome duplicado
                    }

                    // 3) Inserir container
                    string insertQuery = @"
                        INSERT INTO container
                        ([resource-name], [res-type], [creation-datetime], [application-resource-name])
                        VALUES (@resourceName, @resType, @creationDatetime, @applicationName)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@resourceName", container.ResourceName);
                        cmd.Parameters.AddWithValue("@resType", container.ResType);
                        cmd.Parameters.AddWithValue("@creationDatetime", container.CreationDatetime);
                        cmd.Parameters.AddWithValue("@applicationName", container.ApplicationResourceName);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0)
                            return InternalServerError();
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
        */

        // PUT: api/container/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/container/5
        public void Delete(int id)
        {
        }
    }
}
