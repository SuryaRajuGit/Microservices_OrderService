using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class NewWishListProduct
    {
        public Guid ProductId { get; set; }

        public string Name { get; set; }

        public Guid CategoryId { get; set; }
    }
}
