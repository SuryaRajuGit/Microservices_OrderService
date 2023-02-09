using Order_Service.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entity.Model
{
    public class Product : BaseModel
    {
        ///<summary>
        /// Cart id
        ///</summary>
        public Guid CartId { get; set; }
        public Cart Cart { get; set; }

        ///<summary>
        /// Quantity 
        ///</summary>
        public int Quantity { get; set; }

        ///<summary>
        /// product id
        ///</summary>
        public Guid ProductId { get; set; }
    }
}
