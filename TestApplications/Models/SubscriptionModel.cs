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
    internal class SubscriptionModel
    {
        [JsonProperty("resource-name")]
        public string ResourceName { get; set; }

        [JsonProperty("res-type")]
        public string ResType { get; set; } = "subscription";

        [JsonProperty("creation-datetime")]
        public DateTime CreationDatetime { get; set; }

        [JsonProperty("container-resource-name")]
        public virtual string ContainerResourceName { get; set; }

        [JsonProperty("evt")]
        public int Evt { get; set; }

        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }
    }
}
