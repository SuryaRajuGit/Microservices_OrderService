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
        ///<summary>
        ///product id
        ///</summary>
        [Required]
        [JsonProperty(PropertyName = "product_id")]
        public Guid ProductId { get; set; }

        ///<summary>
        /// quantity of the product
        ///</summary>
        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; set; }

        ///<summary>
        /// category id
        ///</summary>
        [Required]
        [JsonProperty(PropertyName = "category_id")]
        public Guid CategoryId { get; set; }

        ///<summary>
        /// User Address city
        ///</summary>
        [Required]
        [JsonProperty(PropertyName = "address_id")]
        public Guid AddressId { get; set; }

        [Required]
        [JsonProperty(PropertyName = "payment_id")]
        public Guid PaymentId { get; set; }
    }
}
