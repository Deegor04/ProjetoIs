using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProjetoIs.Controllers
{
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

        // POST: api/container
        public void Post([FromBody]string value)
        {
        }

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
