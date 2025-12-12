using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApplications.Models
{
    internal class SubscriptionModel
    {
        public string ResourceName { get; set; }
        public string ResType { get; set; } = "subscription";
        public string Evt { get; set; }
        public string Endpoint { get; set; }
    }
}
