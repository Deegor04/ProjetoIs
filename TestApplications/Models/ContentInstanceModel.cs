using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApplications.Models
{
    internal class ContentInstanceModel
    {
        public string ResourceName { get; set; }
        public string ResType { get; set; } = "contentInstance";
        public string ContentType { get; set; }
        public string Content { get; set; }
    }
}
