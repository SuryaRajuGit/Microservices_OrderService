using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class QuantityResponse
    {
        ///<summary>
        /// quantity of the product 
        ///</summary>
        public string type  { get; set; }

        ///<summary>
        /// description of the product
        ///</summary>
        public string description { get; set; }
    }
}
