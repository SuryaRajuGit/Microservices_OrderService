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

        [JsonProperty(PropertyName = "payment_id")]
        public Guid PaymentId { get; set; }

        [JsonProperty(PropertyName = "address_id")]
        public Guid AddressId { get; set; }
    }
}
