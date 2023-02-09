using Order_Service.Entities.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entity.Model
{
    public class Bill : BaseModel
    {
        ///<summary>
        /// id of the bill
        ///</summary>
        public int Id { get; set; }

        ///<summary>
        /// Cart id
        ///</summary>
        public Guid CartId { get; set; }
        public Cart Cart { get; set; }

        ///<summary>
        /// Order value
        ///</summary>
        public float OrderValue { get; set; }

        ///<summary>
        /// Payment id
        ///</summary>
        public Guid PaymentId { get; set; }

        ///<summary>
        /// Address id
        ///</summary>
        public Guid AddressId { get; set; }
    }
}
