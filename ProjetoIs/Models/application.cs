using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
//using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Web;


namespace ProjetoIs.Models
{
    public class application
    {
        [JsonProperty("resource-name")]
        [Column("resource-name")]

        public string ResourceName { get; set; }

        [JsonProperty("res-type")]
        [Column("res-type")]
        public string ResType { get; set; }

        [JsonProperty("creation-datetime")]
        [Column("creation-datetime")] // checkar

        public DateTime CreationDatetime { get; set; }
    }
}