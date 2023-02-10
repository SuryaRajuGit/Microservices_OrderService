using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class ProductDetailsBodyDTO
    {
        public int BillId { get; set; }

        public List<Guid> ProductIds { get; set; }
    }
}
