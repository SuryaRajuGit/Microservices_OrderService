using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class Payment
    {
        ///<summary>
        /// Card id
        ///</summary>
        public Guid Id { get; set; }

        ///<summary>
        /// Forgein key User id
        ///</summary>
        public Guid UserId { get; set; }

        ///<summary>
        /// Name of the user on the card
        ///</summary>
        public string CardHolderName { get; set; }

        ///<summary>
        /// Card Number
        ///</summary>
        public string CardNo { get; set; }

        ///<summary>
        /// Cart Expiry date.
        ///</summary>
        public string ExpiryDate { get; set; }
    }
}
