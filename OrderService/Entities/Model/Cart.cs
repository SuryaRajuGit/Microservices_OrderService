using Order_Service.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entity.Model
{
    public class Cart : BaseModel
    {
      
        public Guid UserId { get; set; }

        public int BillNo { get; set; }

        public ICollection<Bill> Bill { get; set; }

        public ICollection<Product> Product { get; set; }
    }
}
