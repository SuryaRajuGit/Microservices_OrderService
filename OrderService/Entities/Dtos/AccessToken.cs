using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class AccessToken
    {
        public string Token { get; set; }

        public string TokenType { get; set; }
    }
}
