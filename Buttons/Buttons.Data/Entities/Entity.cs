using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buttons.Data.Entities
{
    public abstract class Entity
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        public string UserId { get; set; }

        public EntityType EntityType { get; set; }

        [JsonProperty(PropertyName ="_ts")]
        public int Timestamp { get; set; }
    }
}
