using Newtonsoft.Json;
using Order_Service.Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class OrderResponseDTO
    {
        ///<summary>
        /// Bill no
        ///</summary>
        [JsonProperty(PropertyName = "bill_no")]
        public int BillNo { get; set; }

        ///<summary>
        /// order value
        ///</summary>
        [JsonProperty(PropertyName = "order_value")]
        public float OrderValue  { get; set; }

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

        public List<BillProductsDTO> Product { get; set; }

    }
}
