using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entity.Model
{
    public class Payment
    {
        public Guid Id { get; set; }

        public int BillId { get; set; }
        public Cart BillNo { get; set; }

        public float OrderValue { get; set; }

        public string PaymentType { get; set; }
    }
}
