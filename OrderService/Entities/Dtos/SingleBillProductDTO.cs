using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class SingleBillProductDTO
    {
        ///<summary>
        /// Bill no
        ///</summary>
        [JsonProperty(PropertyName = "bill_no")]
        public int BillId { get; set; }

        ///<summary>
        /// order value
        ///</summary>
        [JsonProperty(PropertyName = "order_value")]
        public float OrderValue { get; set; }

        ///<summary>
        /// Address id
        ///</summary>
        [JsonProperty(PropertyName = "address_id")]
        public Guid AddressId { get; set; }

        ///<summary>
        /// Payment id
        ///</summary>
        [JsonProperty(PropertyName = "payment_id")]
        public Guid PaymentId { get; set; }

        ///<summary>
        /// Product id
        ///</summary>
        [JsonProperty(PropertyName = "product")]
        public BillProductsDTO Product { get; set; }
    }
}
