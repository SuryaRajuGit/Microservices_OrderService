using Microsoft.AspNetCore.Mvc;
using Order_Service.Contracts;
using Order_Service.Entity.DTo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Order_Service.Entities.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace Order_Service.Controllers
{
    [Authorize(AuthenticationSchemes = Constants.Bearer)]
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
            try
            {
                ErrorDTO isTheProductExists = await _orderService.IsTheproductExists(productToCartDTO.ProductId,productToCartDTO.CategoryId);
                if (isTheProductExists != null)
                {
                    return StatusCode(404, isTheProductExists);
                }
            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
            _orderService.AddProduct(productToCartDTO);
            return Ok("Product added to cart sucessfully");
        }
        [HttpPost]
        [Route("api/cart/create")]
        [AllowAnonymous]
        public void CreateCart([FromBody] string id)
        {
            _orderService.CreateCart(Guid.Parse(id));
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
            Guid id = _orderService.GetWishListId(newWwishListProduct.Name);
            return StatusCode(201,id);
        }
        [HttpDelete]
        [Route("api/user/{id}")]
        public void DeleteUser([FromRoute] Guid id)
        {
             _orderService.DeleteUserData(id);
        }

        [HttpDelete]
        [Route("api/wish-list/{id}")]
        [Authorize(Roles = "User,Admin")]
        public IActionResult DeleteWishList([FromRoute] Guid id)
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
            ErrorDTO isWishListExist = _orderService.CheckWishList(wishListProduct.WishListId);
            if (isWishListExist != null)
            {
                return StatusCode(404, isWishListExist);
            }
            ErrorDTO saveProductWishList = _orderService.SaveProductWishList(wishListProduct);
            if (saveProductWishList != null)
            {
                return StatusCode(409, saveProductWishList);
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
            if (deleteProductWishList != null)
            {
                return StatusCode(404, deleteProductWishList);
            }
            return Ok("Product removed from wish list successfully.");
        }
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [Route("api/cart/wish-list")]
        public IActionResult MoveProductToCart([FromBody] WishListToCart wishListToCart)
        {
            if (!ModelState.IsValid)
            {
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO isProductExistsInCart = _orderService.IsProductExistsInCart(wishListToCart);
            if (isProductExistsInCart != null)
            {
                return StatusCode(404, isProductExistsInCart);
            }
            return Ok("Product moved from wish list to cart Successfully");
        }
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [Route("api/check-out/product")]
        public async Task<ActionResult> CheckOutProduct([FromBody] SingleProductCheckOutDTO singleProductCheckOutDTO)
        {
            if (!ModelState.IsValid)
            {
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            try
            {
                ErrorDTO isProductExist = await _orderService.IsTheproductExists(singleProductCheckOutDTO.ProductId,singleProductCheckOutDTO.CategoryId);
                if (isProductExist != null)
                {
                    return StatusCode(404, isProductExist);
                }
                ErrorDTO response = await _orderService.IsPurchaseDetailsExist(singleProductCheckOutDTO);
                if(response != null)
                {
                    return StatusCode(404, isProductExist);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }

        //    try
        //    {
                CheckOutResponse checkOutResponse = await _orderService.CheckOut(singleProductCheckOutDTO);
                return Ok(checkOutResponse);
         //   }
         //   catch (Exception ex)
         //   {
          //      return StatusCode(404, ex.Message);
          //  };
        }
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [Route("api/check-out/cart")]
        public async Task<ActionResult> CheckOutCart([FromBody] CheckOutCart checkOutCart)
        {
            if (!ModelState.IsValid)
            {
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            try
            {
                ErrorDTO response =await _orderService.IsPurchaseDetailsExist(checkOutCart);
                if (response != null)
                {
                    return StatusCode(404, response);
                }
            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
            
            try
            {
                ErrorDTO response = await _orderService.IsQunatityLeft();
                if (response != null)
                {
                    return StatusCode(404, response);
                }
            }
            catch (Exception ex)
            {
                StatusCode(500, ex.Message);
            }
            CheckOutResponse checkOutProduct =await _orderService.CheckOutCart(checkOutCart);           
            if(checkOutProduct == null)
            {
                return StatusCode(201,"Cart is Empty");
            }
            return Ok(checkOutProduct);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        [Route("api/cart")]
        public async Task<ActionResult<List<ProductDTO>>> GetProductsInCart()
        {
            ErrorDTO isUserExist = _orderService.IsUserExist();
            if(isUserExist != null)
            {
                return StatusCode(404,isUserExist);
            }
            try
            {
                List<ProductDTO> products = await _orderService.GetProductsInCart();
                if (products == null)
                {
                    return StatusCode(204, "Cart is Empty");
                }
                return Ok(products);
            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        [Route("api/wish-list/{id}")]
        public async Task<ActionResult<List<WishListProductDTO>>> GetWishListProducts([FromRoute] Guid id)
        {
            ErrorDTO isWishListExist = _orderService.IsWishListExist(id);
            if(isWishListExist != null)
            {
                return StatusCode(404, isWishListExist);
            }
            try
            {
                List<WishListProductDTO> response = await _orderService.GetWishListProducts(id);
                if (response == null)
                {
                    return StatusCode(204, "Wish list empty");
                }
                return Ok(response);
            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
            
        }
    }
}
