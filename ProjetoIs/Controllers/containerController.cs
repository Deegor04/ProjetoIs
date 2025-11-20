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
        
        #region post content-instance
        
        #endregion
        

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
