using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace ProjetoIs.Models
{

 /*************
 * 
 * Install-Package Newtonsoft.Json - provalmente tem de fazer isto - depois podem apagar este comentario
 * 
 *************/
    public class subscription : common // o resto das coisas esta na class "common"
    {
        [ForeignKey("container-resource-name")]
        [JsonProperty("container-resource-name")]
        [Column("container-resource-name")]
        public virtual string ContainerResourceName { get; set; }

        [JsonProperty("evt")]
        [Column("evt")]
        public int Evt { get; set; }

        [JsonProperty("endpoint")]
        [Column("endpoint")]
        public string Endpoint { get; set; }

    }
}