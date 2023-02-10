using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class BillProductsResponseDTO
    {
        ///<summary>
        /// Bill id of the product
        ///</summary>
        public int BillId { get; set; }

        ///<summary>
        /// List of products
        ///</summary>
        public List<BillProductsDTO> Products { get; set; }
    }
}
