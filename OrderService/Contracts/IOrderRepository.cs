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
        public void AddProduct(Product product,Guid userId);

        public bool IsProductInCart(Guid id,Guid userId);

        public void UpdateCart(Cart updateCart);

        public bool IsWishListNameExists(WishList wishList);

        public bool DeleteWishList(Guid id);

        public bool SaveProductWishList(Entity.Model.WishListProduct productWishlist);

        public Tuple<string, string> DeleteProductWishList(Guid id, Guid productId);

        public bool IsProductRemoved(Guid id,Guid userId);
        
        public Tuple<string,string> IsProductExistInCart(WishListToCart cartTOWishList, Guid userId);

        public void CheckOut(Cart cart,Guid userId);

        //  public CheckOutResponse CheckOut(ProductToCartDTO productToCartDTO,Guid userId);

        public bool IsCartExist(Guid id);

        public void CreateCart(Cart cart);

        public Cart GetCartProducts(Guid id);

        public int GenerateBillNo(Cart cart, Bill bill, Guid id);

        public bool CheckWishList(Guid wishListId);


        public Cart GetAllProducstInCart(Guid id);

        public List<WishListProduct> GetProductsInWishList(Guid userId, Guid wishListId);

        public Guid GetWishlistId(string name);

        public Cart? GetCart(Guid cartId);

        public int GetBillNo();

        public void DeleteUserData(Guid id);

        public bool IsUserExist(Guid userId);

        public bool IsTheUserExist(Guid userId);

        public List<Bill> GetOrderDetails(Guid userId);

        public Bill GetOrderDetails(Guid userId,int billNo);

        public bool IsOrderIdExist(int id);

        public bool GetCart(UpdateCart cart, Guid id);
    }
}
