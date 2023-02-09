using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class ProductQuantity
    {
        ///<summary>
        /// product quantity
        ///</summary>
        public int Quantity { get; set; }

        ///<summary>
        /// product id 
        ///</summary>
        public Guid Id { get; set; }
    }
}
