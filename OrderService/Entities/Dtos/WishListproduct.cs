using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class WishListproduct
    {
        ///<summary>
        /// wishl list id
        ///</summary>
        [Required]
        [JsonProperty("wish_list_id")]
        public Guid WishListId { get; set; }

        ///<summary>
        /// product id
        ///</summary>
        [Required]
        [JsonProperty("product_id")]
        public Guid ProductId { get; set; }

        ///<summary>
        /// Catgeory id
        ///</summary>
        [Required]
        [JsonProperty("category_id")]
        public Guid CategoryId { get; set; }
    }
}
