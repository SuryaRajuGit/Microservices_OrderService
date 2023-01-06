using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class WishListToCart
    {
        [Required]
        [JsonProperty("wish_list_id")]
        public string WishListId { get; set; }

        [Required]
        [JsonProperty("product_id")]
        public string ProductId { get; set; }
    }
}
