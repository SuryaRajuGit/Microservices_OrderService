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

        public Task<ErrorDTO> IsTheproductExists(Guid id,Guid categoryId);

        public void AddProduct(ProductToCartDTO productToCartDTO);

        public ErrorDTO IsProductInCart(Guid id);

        public ErrorDTO UpdateProductQuantity(UpdateCart updateCart);

        public ErrorDTO IsWishListNameExists(NewWishListProduct newWishListProduct);

        public ErrorDTO DeleteWishList(Guid id);

        public ErrorDTO SaveProductWishList(WishListproduct wishListProduct);

        public ErrorDTO DeleteProductWishList(Guid id, Guid productId);

        public ErrorDTO DeleteProductCart(Guid id);

        public ErrorDTO IsProductExistsInCart(CartToWishList cartTOWishList);
    }
}
