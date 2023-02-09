using Order_Service.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entity.Model
{
    public class WishList : BaseModel
    {
        ///<summary>
        /// user id
        ///</summary>
        public Guid UserId { get; set; }

        ///<summary>
        /// Name of the wish list
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        /// list of wish list 
        ///</summary>
        public ICollection<WishListProduct> WishListProduct { get; set; }
    }
}
