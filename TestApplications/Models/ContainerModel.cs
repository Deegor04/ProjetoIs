using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SomiodPublisher.Models
{
    internal class ContainerModel
    {
        [JsonProperty("resource-name")]
        public string ResourceName { get; set; }

        [JsonProperty("res-type")]
        public string ResType { get; set; } = "container";

        [JsonProperty("creation-datetime")]
        public DateTime CreationDatetime { get; set; }

        [JsonProperty("application-resource-name")]
        public virtual string ApplicationResourceName { get; set; }
    }
}
