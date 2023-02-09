using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Model
{
    public class BaseModel
    {
        ///<summary>
        /// id of the model
        ///</summary>
        public Guid Id { get; set; }

        ///<summary>
        /// created id of the model
        ///</summary>
        public Guid CreateBy { get; set; }

        ///<summary>
        /// updated id of the model
        ///</summary>
        public Guid UpdatedBy { get; set; }

        ///<summary>
        /// Created date
        ///</summary>
        public DateTime CreatedDate { get; set; }

        ///<summary>
        /// Updated date
        ///</summary>
        public DateTime UpdatedDate { get; set; }

        ///<summary>
        /// is avctive of the model
        ///</summary>
        public bool IsActive { get; set; }
    }
}
