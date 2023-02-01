using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entities.Model
{
    public class BaseModel
    {
        public Guid Id { get; set; }

        public Guid CreateBy { get; set; }

        public Guid UpdatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public bool IsActive { get; set; }
    }
}
