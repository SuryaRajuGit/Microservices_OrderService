﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class CheckOutResponseDTO
    {
        public Address Address  { get; set; }

        public Payment Payment { get; set; }

        
    }
}
