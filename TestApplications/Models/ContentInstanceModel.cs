using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomiodPublisher.Models
{
    internal class ContentInstanceModel
    {
        [Key]
        [JsonProperty("resource-name")]
        public string ResourceName { get; set; }

        [JsonProperty("res-type")]
        public string ResType { get; set; } = "contentInstance";

        [JsonProperty("creation-datetime")]
        public DateTime CreationDatetime { get; set; }

        [JsonProperty("container-resource-name")]
        public string ContainerResourceName { get; set; }

        [JsonProperty("content-type")]
        public string ContentType { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
