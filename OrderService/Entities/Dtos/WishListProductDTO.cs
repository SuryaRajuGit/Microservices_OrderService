using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class WishListProductDTO
    {
        ///<summary>
        /// id of the wishlist
        ///</summary>
        public Guid Id { get; set; }

        ///<summary>
        /// name of the wish list
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        /// User Address city
        ///</summary>
        public float Price { get; set; }

        ///<summary>
        /// description of the product
        ///</summary>
        public string Description { get; set; }

        ///<summary>
        /// Asset of the product
        ///</summary>
        public string? Asset { get; set; }

        ///<summary>
        /// Quantity of the product
        ///</summary>
        public string Qunatity { get; set; }
    }
}
