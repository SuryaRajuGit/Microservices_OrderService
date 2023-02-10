using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class ProductDetailsBodyDTO
    {
        ///<summary>
        /// Bill no
        ///</summary>
        public int BillId { get; set; }

        ///<summary>
        /// Product id
        ///</summary>
        public List<Guid> ProductIds { get; set; }
    }
}
