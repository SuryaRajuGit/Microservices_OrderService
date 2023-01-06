using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class ProductQuantity
    {
        public int Quantity { get; set; }

        public Guid Id { get; set; }
    }
}
