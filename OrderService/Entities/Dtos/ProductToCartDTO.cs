
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class ProductToCartDTO
    {
        ///<summary>
        /// product id
        ///</summary>
        [Required]
        [JsonProperty(PropertyName = "product_id")]
        public Guid ProductId { get; set; }

        ///<summary>
        /// product quantity
        ///</summary>
        [Required]
        [JsonProperty( PropertyName = "quantity")]
        public int Quantity { get; set; }
    }
}
