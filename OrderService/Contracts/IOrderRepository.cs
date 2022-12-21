using Order_Service.Entities.Dtos;
using Order_Service.Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Contracts
{
    public interface IOrderRepository
    {
        public void AddProduct(Cart cart);

        public bool IsProductInCart(Guid id,Guid userId);

        public bool IsProductAdded(Cart updateCart);

        public bool IsWishListNameExists(WishList wishList);

        public bool DeleteWishList(Guid id);

        public Tuple<string, string> SaveProductWishList(Entity.Model.WishListProduct productWishlist);

        public Tuple<string, string> DeleteProductWishList(Guid id, Guid productId);

        public bool IsProductRemoved(Guid id,Guid userId);
    }
}
