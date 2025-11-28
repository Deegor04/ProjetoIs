using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
//using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace ProjetoIs.Models
{
   
    public class content_instance : common
    {
        [ForeignKey("container-resource-name")]
        [JsonProperty("container-resource-name")]
        [Column("container-resource-name")]
        public string ContainerResourceName { get; set; }

        [JsonProperty("content-type")]
        [Column("content-type")]
        public string ContentType { get; set; }

        [JsonProperty("content")]
        [Column("content")]
        public string Content { get; set; }
    }
}