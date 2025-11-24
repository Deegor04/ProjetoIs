using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [JsonProperty("application-resource-name")]
        [Column("application-resource-name")]
        public virtual string ApplicationResourceName { get; set; }

        //public virtual List<content_instance> Content_Instance_List { get; set; } = new List<content_instance>();

        //public virtual List<subscription> subscription_list { get; set; } = new List<subscription>(); 
    }
}