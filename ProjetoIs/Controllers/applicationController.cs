using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProjetoIs.Controllers
{
    public class applicationController : ApiController
    {
        // GET: api/application
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/application/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/application
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/application/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/application/5
        public void Delete(int id)
        {
        }
    }
}
