using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class ProductPrice
    {
        ///<summary>
        /// Product price
        ///</summary>
        public float Price { get; set; }

        ///<summary>
        /// id of the product
        ///</summary>
        public Guid Id { get; set; }
    }
}
