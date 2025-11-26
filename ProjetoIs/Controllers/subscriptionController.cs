using ProjetoIs.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProjetoIs.Controllers
{
    [RoutePrefix("api/somiod/{applicationName}/{containerName}/subs/{subName}")]
    public class subscriptionController : ApiController
    {
        string connectionString = ConfigurationManager.ConnectionStrings["ProjetoIs.Properties.Settings.ConnectionString"].ConnectionString;

        [HttpGet]
        [Route("{appName}/{containerName}/subs/{subName}")]
        public IHttpActionResult GetSubscription(string appName, string containerName, string subName)
        {
            var subscription = GetSubscriptionFromDb(appName, containerName, subName);
            if (subscription == null)
            {
                return NotFound();
            }

            return Ok(subscription);
        }

        public subscription GetSubscriptionFromDb(global::System.String appName, global::System.String containerName, global::System.String subName)
        {
            const string connDb = @"";
        }


    }
}
