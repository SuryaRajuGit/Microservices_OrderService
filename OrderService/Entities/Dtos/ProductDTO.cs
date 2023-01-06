using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class ProductDTO
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "price")]
        public float Price { get; set; }

        [JsonProperty(PropertyName = "asset")]
        public byte[] Asset { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "quantity")]
        public string Quantity { get; set; }
    }
}
