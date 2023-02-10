using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class BillProductsResponseDTO
    {

        public int BillId { get; set; }

        public List<BillProductsDTO> Products { get; set; }
    }
}
