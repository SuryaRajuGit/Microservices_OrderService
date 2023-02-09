using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Dtos
{
    public class AccessToken
    {
        ///<summary>
        /// token 
        ///</summary>
        public string Token { get; set; }

        ///<summary>
        ///Token type
        ///</summary>
        public string TokenType { get; set; }
    }
}
