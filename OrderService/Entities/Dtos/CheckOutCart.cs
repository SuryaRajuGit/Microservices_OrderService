using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class CheckOutCart
    {
        ///<summary>
        /// Payment id
        ///</summary>
        [JsonProperty(PropertyName = "payment_id")]
        public Guid PaymentId { get; set; }

        ///<summary>
        /// address id
        ///</summary>
        [JsonProperty(PropertyName = "address_id")]
        public Guid AddressId { get; set; }
    }
}
