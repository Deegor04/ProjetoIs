using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace ProjetoIs.Models
{
    public class content_instance : common
    {
        [ForeignKey("container-resource-name")]
        [JsonProperty("container-resource-name")]
        [Column("container-resource-name")]
        public virtual string containerResourceName { get; set; }

        [JsonProperty("content-type")]
        [Column("content-type")]
        public String ContentType { get; set; }

        [JsonProperty("content")]
        [Column("content")]
        public String Content { get; set; }
    }
}