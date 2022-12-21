using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class WishListproduct
    {
        public Guid WishListId { get; set; }

        public Guid ProductId { get; set; }

        public Guid CategoryId { get; set; }
    }
}
