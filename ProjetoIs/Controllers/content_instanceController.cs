using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProjetoIs.Controllers
{
    [RoutePrefix("api/somiod/{applicationName}/{containerName}")]
    public class content_instanceController : ApiController
    {
        string connectionString = ConfigurationManager.ConnectionStrings["ProjetoIs.Properties.Settings.ConnectionString"].ConnectionString;
        // GET: api/content_instance
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/content_instance/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/content_instance
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/content_instance/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/content_instance/5
        public void Delete(int id)
        {
        }
    }
}
