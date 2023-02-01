using Order_Service.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entity.Model
{
    public class Product : BaseModel
    {

        public Guid CartId { get; set; }
        public Cart Cart { get; set; }

        public int Quantity { get; set; }

        public Guid ProductId { get; set; }
    }
}
