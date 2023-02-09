using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class NewWishListProduct
    {
        ///<summary>
        /// product id
        ///</summary>
        [Required]
        [JsonProperty(PropertyName ="product_id")]
        public Guid ProductId { get; set; }

        ///<summary>
        /// Product name
        ///</summary>
        [Required]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
