
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
        [Required]
        [JsonProperty(PropertyName = "product_id")]
        public Guid ProductId { get; set; }

        [Required]
        [JsonProperty(PropertyName = "category_id")]
        public Guid CategoryId { get; set; }

        [Required]
        [JsonProperty( PropertyName = "quantity")]
        public int Quantity { get; set; }
    }
}
