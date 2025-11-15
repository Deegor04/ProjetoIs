using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace ProjetoIs.Models
{
    public class common
    {
        [Key]
        [JsonProperty("resource-name")]
        [Column("resource-name")]
        public string ResourceName { get; set; }

        [JsonProperty("res-type")]
        [Column("res-type")]
        public string ResType { get; set; }

        [JsonProperty("creation-datetime")]
        [Column("creation-datetime")]
        public DateTime CreationDatetime { get; set; }
    }
}