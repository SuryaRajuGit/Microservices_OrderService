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
        ///<summary>
        /// Adds product to cart
        ///</summary>
        public void AddProduct(Product product,Guid userId);

        ///<summary>
        /// Checks if the product exist in the cart
        ///</summary>
        public bool IsProductInCart(Guid id,Guid userId);

        ///<summary>
        /// updates cart details and return bool
        ///</summary>
        public void UpdateCart(Cart updateCart);

        ///<summary>
        /// Checks wish-list name exist or not
        ///</summary>
        public bool IsWishListNameExists(WishList wishList);

        ///<summary>
        /// Deletes wish-list 
        ///</summary>
        public bool DeleteWishList(Guid id);

        //<summary>
        /// Saves product wish-list 
        ///</summary>
        public bool SaveProductWishList(Entity.Model.WishListProduct productWishlist);

        ///<summary>
        /// Deletes product in wish-list
        ///</summary>
        public Tuple<string, string> DeleteProductWishList(Guid id, Guid productId);

        ///<summary>
        /// Removes product from user cart
        ///</summary>
        public bool IsProductRemoved(Guid id,Guid userId);

        ///<summary>
        /// Checks if the product exist in the cart or not
        ///</summary>
        public Tuple<string,string> IsProductExistInCart(WishListToCart cartTOWishList, Guid userId);

        ///<summary>
        /// updates cart details 
        ///</summary>
        public void CheckOut(Cart cart,Guid userId);

        ///<summary>
        /// checks cart id exist or not
        ///</summary>
        public bool IsCartExist(Guid id);

        ///<summary>
        /// Create cart for user
        ///</summary>
        public void CreateCart(Cart cart);

        ///<summary>
        /// returns Cart model 
        ///</summary>
        public Cart GetCartProducts(Guid id);

        ///<summary>
        /// Generates bill no 
        ///</summary>
        public int GenerateBillNo(Cart cart, Bill bill, Guid id);

        ///<summary>
        /// Checks wish-list id exist or not
        ///</summary>
        public bool CheckWishList(Guid wishListId);

        ///<summary>
        /// returns all products in the user cart
        ///</summary>
        public Cart GetAllProducstInCart(Guid id);

        ///<summary>
        /// Returns all products in wish-list
        ///</summary>
        public List<WishListProduct> GetProductsInWishList(Guid userId, Guid wishListId);

        ///<summary>
        /// Resturns wish-list id
        ///</summary>
        public Guid GetWishlistId(string name,Guid id);

        ///<summary>
        /// Return Cart Model
        ///</summary>
        public Cart? GetCart(Guid cartId);

        ///<summary>
        /// Generates bill number 
        ///</summary>
        public int GetBillNo();

        ///<summary>
        /// Deletes user details 
        ///</summary>
        public void DeleteUserData(Guid id);

        ///<summary>
        /// checks user id exist or not
        ///</summary>
        public bool IsUserExist(Guid userId);

        ///<summary>
        /// checks user id exist or not
        ///</summary>
        public bool IsTheUserExist(Guid userId);

        ///<summary>
        ///Returns list of order details
        ///</summary>
        public List<Bill> GetOrderDetails(Guid userId);

        ///<summary>
        ///Returns list of order details
        ///</summary>
        public Bill GetOrderDetails(Guid userId,int billNo);

        ///<summary>
        /// Checks order id exist or not
        ///</summary>
        public bool IsOrderIdExist(int id);

        ///<summary>
        /// updates cart details and return bool
        ///</summary>
        public bool GetCart(UpdateCart cart, Guid id);
    }
}
