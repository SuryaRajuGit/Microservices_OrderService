using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entity.DTo
{
    public class ErrorDTO
    {
        ///<summary>
        /// Error type
        ///</summary>
        public string type { get; set; }

        ///<summary>
        /// Status code
        ///</summary>
        public string statusCode { get; set; }
        ///<summary>
        /// Error description
        ///</summary>
        public string message { get; set; }
    }
}
