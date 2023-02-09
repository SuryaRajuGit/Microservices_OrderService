using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class ProductDTO
    {
        ///<summary>
        /// id of the product
        ///</summary>
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        ///<summary>
        /// name of the product
        ///</summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        ///<summary>
        /// price of the product
        ///</summary>
        [JsonProperty(PropertyName = "price")]
        public float Price { get; set; }

        ///<summary>
        /// Asset of the product
        ///</summary>
        [JsonProperty(PropertyName = "asset")]
        public string? Asset { get; set; }

        ///<summary>
        /// Description of the product
        ///</summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        ///<summary>
        /// Quantity of the product
        ///</summary>
        [JsonProperty(PropertyName = "quantity")]
        public string Quantity { get; set; }
    }
}
