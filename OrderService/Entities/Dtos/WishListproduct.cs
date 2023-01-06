﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class WishListproduct
    {
        [Required]
        [JsonProperty("wishList_id")]
        public Guid WishListId { get; set; }

        [Required]
        [JsonProperty("product_id")]
        public Guid ProductId { get; set; }

        [Required]
        [JsonProperty("category_id")]
        public Guid CategoryId { get; set; }
    }
}
