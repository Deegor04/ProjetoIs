using ProjetoIs.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Configuration;

namespace ProjetoIs.Controllers
{
    [RoutePrefix("api/somiod/")]
    public class subscriptionController : ApiController
    {
        string connectionString = Properties.Settings.Default.ConnectionString;

        [HttpGet]
        [Route("{applicationName}/{containerName}/subs/{subName}")]
        public IHttpActionResult GetSubscription(string appName, string containerName, string subName)
        {
            var subscription = GetSubscriptionFromDb(appName, containerName, subName);
            if (subscription == null)
            {
                return NotFound();
            }

            return Ok(subscription);
        }

        public subscription GetSubscriptionFromDb(string appName, string containerName, string subName)
        {
            const string connDb = @"SELECT s.[resource-name], s.[creation-datetime], s.[container-resource-name],
                s.[res-type], s.[evt], s.[endpoint]
                FROM [subscription] s JOIN [container] c ON c.[resource-name] = s.[container-resource-name]
                JOIN [application] a ON a.[resource-name] = c.[application-resource-name]
                WHERE a.[resource-name] = @AppName
                AND c.[resource-name] = @ContainerName
                AND s.[resource-name] = @SubName";

            var conn = new SqlConnection(connectionString);
            var cmd = new SqlCommand(connDb, conn);

            cmd.Parameters.AddWithValue("@AppName", appName);
            cmd.Parameters.AddWithValue("@ContainerName", containerName);
            cmd.Parameters.AddWithValue("@SubName", subName);
        }


    }
}
