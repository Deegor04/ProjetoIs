using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;


namespace ProjetoIs.Models
{
    public class application
    {
        [JsonPropertyName("resource-name")]
        [Column("resource_name")]

        public string ResourceName { get; set; }

        [JsonPropertyName("res-type")]
        [Column("res-type")]
        public string ResType { get; set; }

        [JsonPropertyName("creation-datetime")]
        [Column("creation_datetime")]

        public DateTime CreationDatetime { get; set; }
    }
}