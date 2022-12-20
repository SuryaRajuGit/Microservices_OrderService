using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entity.Model
{
    public class WishListProduct
    {
        public Guid Id { get; set; }

        public Guid WishListId { get; set; }
        public WishList WishList { get; set; }

        public Guid ProductId { get; set; } 

    }
}
