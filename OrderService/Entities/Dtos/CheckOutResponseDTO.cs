using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class CheckOutResponseDTO
    {
        ///<summary>
        /// Address of the user
        ///</summary>
        public Address Address  { get; set; }

        ///<summary>
        /// Payment of the user
        ///</summary>
        public Payment Payment { get; set; }

        
    }
}
