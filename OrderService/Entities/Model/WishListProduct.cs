using Order_Service.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entity.Model
{
    public class WishListProduct : BaseModel
    {

        ///<summary>
        /// wish list id
        ///</summary>
        public Guid WishListId { get; set; }
        public WishList WishList { get; set; }

        ///<summary>
        /// product id
        ///</summary>
        public Guid ProductId { get; set; } 

    }
}
