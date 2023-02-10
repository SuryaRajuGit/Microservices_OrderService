using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class BillProductsDTO
    {
        ///<summary>
        /// Bill id
        ///</summary>
        public Guid Id { get; set; }

        ///<summary>
        /// Name of the product
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        /// Description of the product
        ///</summary>
        public string Description { get; set; }

        ///<summary>
        /// Asset of the product
        ///</summary>
        public string Asset { get; set; }

        ///<summary>
        /// Quantity of the product
        ///</summary>
        public int Quantity { get; set; }

    }
}
