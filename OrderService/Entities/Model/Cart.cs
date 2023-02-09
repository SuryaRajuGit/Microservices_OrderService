using Order_Service.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entity.Model
{
    public class Cart : BaseModel
    {
        ///<summary>
        /// user id
        ///</summary>
        public Guid UserId { get; set; }

        ///<summary>
        /// Bill no
        ///</summary>
        public int BillNo { get; set; }

        ///<summary>
        /// List of bill models
        ///</summary>
        public ICollection<Bill> Bill { get; set; }

        ///<summary>
        /// list of product models
        ///</summary>
        public ICollection<Product> Product { get; set; }
    }
}
