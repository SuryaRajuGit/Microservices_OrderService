using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class CheckOutResponse
    {
        [JsonProperty(PropertyName = "bill_no")]
        public int BillNo { get; set; }

        [JsonProperty(PropertyName = "product_name")]
        public float OrderValue { get; set; }

        [JsonProperty(PropertyName = "payment_type")]
        public string PaymentType { get; set; }

        [JsonProperty(PropertyName = "shipping_address")]
        public string ShippingAddress { get; set; }
    }
}
