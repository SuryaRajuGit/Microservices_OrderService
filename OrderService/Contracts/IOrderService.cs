using Microsoft.AspNetCore.Mvc.ModelBinding;
using Order_Service.Entities.Dtos;
using Order_Service.Entity.DTo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Contracts
{
    public interface IOrderService
    {
        ///<summary>
        /// checks the properties are valid or not 
        ///</summary>
        public ErrorDTO ModelStateInvalid(ModelStateDictionary ModelState);

        ///<summary>
        /// checks products in the cart exist or not
        ///</summary>
        public Task<ErrorDTO> CartProductsExist(List<ProductToCartDTO> productToCartDTOs);

        ///<summary>
        /// checks if the product exist or not 
        ///</summary>
        public Task<ErrorDTO> IsTheproductExists(Guid id);

        ///<summary>
        /// adds product to user cart
        ///</summary>
        public void AddProduct(ProductToCartDTO productToCartDTO);

        ///<summary>
        /// adds product to user cart
        ///</summary>
        public ErrorDTO IsProductInCart(Guid id);

        ///<summary>
        /// updates product quantity in cart
        ///</summary>
        public ErrorDTO UpdateProductQuantity(UpdateCart updateCart);

        ///<summary>
        /// checks wish-list name already exist or not
        ///</summary>
        public ErrorDTO IsWishListNameExists(NewWishListProduct newWishListProduct);

        ///<summary>
        /// Deletes wish-list 
        ///</summary>
        public ErrorDTO DeleteWishList(Guid id);

        ///<summary>
        /// saves new wish-list 
        ///</summary>
        public ErrorDTO SaveProductWishList(WishListproduct wishListProduct);

        ///<summary>
        /// Deletes product in user wish-list
        ///</summary>
        public ErrorDTO DeleteProductWishList(Guid id, Guid productId);

        ///<summary>
        /// Deletes product in cart if the product id doesnt exist return not found
        ///</summary>
        public ErrorDTO DeleteProductCart(Guid id);

        ///<summary>
        /// Checks if the product exist in the cart or not
        ///</summary>
        public ErrorDTO IsProductExistsInCart(WishListToCart cartTOWishList);

        ///<summary>
        /// removes all products from cart and updates quantity in product service
        ///</summary>
        public Task<int> CheckOut(SingleProductCheckOutDTO singleProductCheckOutDTO);

        ///<summary>
        /// Creates cart for user
        ///</summary>
        public void CreateCart(Guid id);

        ///<summary>
        /// Checks quantity of the product in product service and return error 
        ///</summary>
        public Task<ErrorDTO> IsQunatityLeft(Guid id,int quantity);

        ///<summary>
        /// Checks cart id exist or not
        ///</summary>
        public ErrorDTO IsCartIdExist(Guid id);


        ///<summary>
        /// removes all products from cart and updates quantity in product service
        ///</summary>
        public Task<int?> CheckOutCart(CheckOutCart checkOutCart);

        ///<summary>
        /// checks wish-list name exist or not
        ///</summary>
        public ErrorDTO CheckWishList(Guid wishListId);

        ///<summary>
        /// retruns all products details in the user cart
        ///</summary>
        public Task<List<ProductDTO>> GetProductsInCart();

        ///<summary>
        /// Checks wish-list id exist or not
        ///</summary>
        public ErrorDTO IsWishListExist(Guid id);

        ///<summary>
        /// returns list or wish list products with user id
        ///</summary>
        public Task<List<WishListProductDTO>> GetWishListProducts(Guid id);

        ///<summary>
        /// returns created wish-list id 
        ///</summary>
        public Guid GetWishListId(string name);

        ///<summary>
        /// Cheks if the purchase details exist in the user microservice
        ///</summary>
        public Task<ErrorDTO> IsPurchaseDetailsExist(CheckOutCart checkOutCart);

        ///<summary>
        /// Cheks if the purchase details exist in the user microservice
        ///</summary>
        public Task<ErrorDTO> IsPurchaseDetailsExist(SingleProductCheckOutDTO singleProductCheckOutDTO);

        ///<summary>
        /// Deletes user account 
        ///</summary>
        public void  DeleteUserData(Guid id);

        ///<summary>
        /// Checks user exist or not
        ///</summary>
        public ErrorDTO IsUserExist();

        ///<summary>
        /// retusn order details 
        ///</summary>
        public OrderResponseDTO GetOrderDetails(int billNo);

        ///<summary>
        /// Checks order id exist or not
        ///</summary>
        public ErrorDTO IsOrderIdExist(int id);

        ///<summary>
        /// retusn order details 
        ///</summary>

        public Task<List<OrderResponseDTO>> GetOrderDetails();




    }
}
