using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entity.Model
{
    public class Cart
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public ICollection<Payment> BillNo { get; set; }

        public ICollection<Product> Product { get; set; }
    }
}
