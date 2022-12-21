using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class UpdateCart
    {
        public Guid ProductId { get; set; }

        public Guid CartId { get; set; }

        public int Quantity { get; set; }
    }
}
