using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace ProjetoIs.Models
{
    public class container : common
    {
        [ForeignKey("application-resource-name")]
        public virtual application ApplicationResourceName { get; set; }
    }
}