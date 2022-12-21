using Microsoft.AspNetCore.Mvc;
using Order_Service.Contracts;
using Order_Service.Entity.DTo;
using Order_Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Order_Service.Entities.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace Order_Service.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [Route("api/cart")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult> AddProductCart([FromBody] ProductToCartDTO productToCartDTO)
        {
            if (!ModelState.IsValid)
            {
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;
            ErrorDTO isTheProductExists = await _orderService.IsTheproductExists(productToCartDTO.CategoryId, productToCartDTO.ProductId);
            if (isTheProductExists != null)
            {
                return StatusCode(404, isTheProductExists);
            }
            _orderService.AddProduct(productToCartDTO);
            return Ok();
        }

        [HttpDelete]
        [Route("api/cart/product/{id}")]
        [Authorize(Roles = "User,Admin")]
        public IActionResult DeleteProductCart([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO isProductInCart = _orderService.IsProductInCart(id);
            if (isProductInCart != null)
            {
                return NotFound(isProductInCart);
            }
            return Ok("Product removed from Cart Successfully");
        }

        [HttpPut]
        [Route("api/cart")]
        [Authorize(Roles = "User,Admin")]
        public IActionResult UpdateQuantity([FromBody] UpdateCart updateCart)
        {
            if (!ModelState.IsValid)
            {
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO updateProductQunatity = _orderService.UpdateProductQuantity(updateCart);
            if (updateProductQunatity != null)
            {
                return StatusCode(404, updateProductQunatity);
            }
            return Ok("Product Qunatity Updated Successfully");
        }

        [HttpPost]
        [Route("api/wish-list")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult> CreateWishListProduct([FromBody] NewWishListProduct newWwishListProduct)
        {
            if (!ModelState.IsValid)
            {
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO isTheProductExists = await _orderService.IsTheproductExists(newWwishListProduct.ProductId, newWwishListProduct.CategoryId);
            if (isTheProductExists != null)
            {
                return StatusCode(404, isTheProductExists);
            }
            ErrorDTO isWishListNameExists = _orderService.IsWishListNameExists(newWwishListProduct);
            if (isWishListNameExists != null)
            {
                return StatusCode(409, isWishListNameExists);
            }
            return Ok("Product Added to Wish List Successfully");
        }
        [HttpDelete]
        [Route("api/wish-list/{id}")]
        [Authorize(Roles = "User,Admin")]
        public IActionResult DeleteWishList([FromBody] Guid id)
        {
            if (!ModelState.IsValid)
            {
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO deleteWishList = _orderService.DeleteWishList(id);
            if (deleteWishList != null)
            {
                return StatusCode(404, deleteWishList);
            }
            return Ok("Wish List deleted Successfully");
        }
        [HttpPost]
        [Route("api/wish-list/product")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult> AddProductWishList([FromBody] WishListproduct wishListProduct)
        {
            if (!ModelState.IsValid)
            {
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO isTheProductExists = await _orderService.IsTheproductExists(wishListProduct.ProductId, wishListProduct.CategoryId);
            if (isTheProductExists != null)
            {
                return StatusCode(404, isTheProductExists);
            }
            ErrorDTO saveProductWishList = _orderService.SaveProductWishList(wishListProduct);
            if(saveProductWishList != null)
            {
                return StatusCode(409,saveProductWishList);
            }
            return Ok("Product added to Wish list Successfully");
        }
        [HttpDelete]
        [Authorize(Roles = "Admin,User")]
        [Route("api/wish-list/{id}/product/{product-id}")]
        public IActionResult DeleteProductInWishList([FromRoute] Guid id, [FromRoute(Name = "product-id")] Guid productId)
        {
            if (!ModelState.IsValid)
            {
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO deleteProductWishList = _orderService.DeleteProductWishList(id, productId);
            if(deleteProductWishList != null)
            {
                return StatusCode(404,deleteProductWishList);
            }
            return Ok("Product Removed from Wish List Successfully");
        }
        [HttpDelete]
        [Authorize(Roles = "Admin,User")]
        [Route("api/cart/product/{id}")]
        public IActionResult DeleteProductInCart([FromRoute]Guid id)
        {
            if (!ModelState.IsValid)
            {
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO deleteProductCart = _orderService.DeleteProductCart(id);
            if(deleteProductCart != null)
            {
                return StatusCode(404,deleteProductCart);
            }
            return Ok("Product removed from cart successfully");
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [Route("api/cart/wish-list")]
        public IActionResult MoveProductToCart([FromBody] CartToWishList cartTOWishList)
        {
            if (!ModelState.IsValid)
            {
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO isProductExistsInCart = _orderService.IsProductExistsInCart(cartTOWishList);
        }

        
    }
}
