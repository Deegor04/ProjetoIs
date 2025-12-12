using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApplications.Models
{
    internal class ContainerModel
    {
        public string ResourceName { get; set; }
        public string ResType { get; set; } = "container";
        public string ApplicationName { get; set; }
    }
}
