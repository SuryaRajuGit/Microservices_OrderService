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
        public ErrorDTO ModelStateInvalid(ModelStateDictionary ModelState);

        public Task<ErrorDTO> CartProductsExist(List<ProductToCartDTO> productToCartDTOs);

        public Task<ErrorDTO> IsTheproductExists(Guid id,Guid categoryId);

        public void AddProduct(ProductToCartDTO productToCartDTO);

        public ErrorDTO IsProductInCart(Guid id);

        public ErrorDTO UpdateProductQuantity(UpdateCart updateCart);

        public ErrorDTO IsWishListNameExists(NewWishListProduct newWishListProduct);

        public ErrorDTO DeleteWishList(Guid id);

        public ErrorDTO SaveProductWishList(WishListproduct wishListProduct);

        public ErrorDTO DeleteProductWishList(Guid id, Guid productId);

        public ErrorDTO DeleteProductCart(Guid id);

        public ErrorDTO IsProductExistsInCart(WishListToCart cartTOWishList);

        public Task<int> CheckOut(SingleProductCheckOutDTO singleProductCheckOutDTO);

        public void CreateCart(Guid id);

        public Task<ErrorDTO> IsQunatityLeft(Guid id,int quantity);

        public ErrorDTO isCartIdExist(Guid id);

        public Task<int?> CheckOutCart(CheckOutCart checkOutCart);

        public ErrorDTO CheckWishList(Guid wishListId);

        public Task<List<ProductDTO>> GetProductsInCart();

        public ErrorDTO IsWishListExist(Guid id);

        public Task<List<WishListProductDTO>> GetWishListProducts(Guid id);

        public Guid GetWishListId(string name);

        public Task<ErrorDTO> IsPurchaseDetailsExist(CheckOutCart checkOutCart);

        public Task<ErrorDTO> IsPurchaseDetailsExist(SingleProductCheckOutDTO singleProductCheckOutDTO);

        public void  DeleteUserData(Guid id);

        public ErrorDTO IsUserExist();

        public List<OrderResponseDTO> GetOrderDetails(int billNo);

        public ErrorDTO IsOrderIdExist(int id);


    }
}
