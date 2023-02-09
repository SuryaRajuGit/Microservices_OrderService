using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class CheckOutResponse
    {
        ///<summary>
        /// bill no 
        ///</summary>
        [JsonProperty(PropertyName = "bill_no")]
        public int BillNo { get; set; }

        ///<summary>
        /// order value 
        ///</summary>
        [JsonProperty(PropertyName = "product_name")]
        public float OrderValue { get; set; }

        ///<summary>
        /// payment type
        ///</summary>
        [JsonProperty(PropertyName = "payment_type")]
        public string PaymentType { get; set; }

        ///<summary>
        /// Shipping address id
        ///</summary>
        [JsonProperty(PropertyName = "shipping_address")]
        public Guid ShippingAddress { get; set; }
    }
}
