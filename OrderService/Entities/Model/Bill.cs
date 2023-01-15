using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entity.Model
{
    public class Bill
    {
       
        public int Id { get; set; }

        public Guid CartId { get; set; }
        public Cart Cart { get; set; }

        public float OrderValue { get; set; }

        public Guid PaymentId { get; set; }

        public Guid AddressId { get; set; }
    }
}
