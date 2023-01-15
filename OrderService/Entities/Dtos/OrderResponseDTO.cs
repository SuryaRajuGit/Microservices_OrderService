using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class OrderResponseDTO
    {
        [JsonProperty(PropertyName = "bill_no")]
        public int BillNo { get; set; }

        [JsonProperty(PropertyName = "order_value")]
        public float OrderValue  { get; set; }

        [JsonProperty(PropertyName = "address_id")]
        public Guid AddressId { get; set; }

        [JsonProperty(PropertyName = "payment_id")]
        public Guid PaymentId { get; set; } 
    }
}
