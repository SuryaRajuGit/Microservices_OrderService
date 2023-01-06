using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class SingleProductCheckOutDTO
    {
        [Required]
        [JsonProperty(PropertyName = "product_id")]
        public Guid ProductId { get; set; }

        [Required]
        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; set; }

        [Required]
        [JsonProperty(PropertyName = "category_id")]
        public Guid CategoryId { get; set; }

        
        [JsonProperty(PropertyName = "address_id")]
        public Guid AddressId { get; set; }

        [Required]
        [JsonProperty(PropertyName = "payment_id")]
        public Guid PaymentId { get; set; }
    }
}
