using Order_Service.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Entity.Model
{
    public class WishList : BaseModel
    {

        public Guid UserId { get; set; }

        public string Name { get; set; }

        public ICollection<WishListProduct> WishListProduct { get; set; }
    }
}
