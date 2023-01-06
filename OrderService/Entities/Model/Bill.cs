using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entity.Model
{
    public class Bill
    {
        public Guid Id { get; set; }

        public int BillNo { get; set; }
        public Cart cart { get; set; }

        public float OrderValue { get; set; }

        public string PaymentType { get; set; }

        public string ShippingAddress { get; set; }
    }
}
