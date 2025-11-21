using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProjetoIs.Controllers
{
    [RoutePrefix("api/somiod/{applicationName}/{containerName}/sub")]
    public class subscriptionController : ApiController
    {
        string connectionString = ConfigurationManager.ConnectionStrings["ProjetoIs.Properties.Settings.ConnectionString"].ConnectionString;

        [HttpGet]
        [Route("")]
        public void Get()
        {
        }
    }
}
