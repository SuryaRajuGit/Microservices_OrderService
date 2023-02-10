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
using Microsoft.Extensions.Logging;

namespace Order_Service.Controllers
{
    [Authorize(AuthenticationSchemes = Constants.Bearer)]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;
        public OrderController(IOrderService orderService , ILogger logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        ///<summary>
        /// Add product to cart
        ///</summary>
        ///<return>Product added to cart successfully</return>
        [HttpPost]
        [Route("api/cart")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult>  AddProductCart([FromBody] ProductToCartDTO productToCartDTO)
        {
            _logger.LogInformation("Adding product to cart started");
            if (!ModelState.IsValid)
            {
                _logger.LogError("Entered wrong data");
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            try
            {
                ErrorDTO isTheProductExists = await _orderService.IsTheproductExists(productToCartDTO.ProductId);
                if (isTheProductExists != null)
                {
                    _logger.LogError("Product id not found");
                    return StatusCode(404, isTheProductExists);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("Product service unavailable");
                return StatusCode(500,ex.Message);
            }
            ErrorDTO response = await _orderService.IsQunatityLeft(productToCartDTO.ProductId,productToCartDTO.Quantity);
            if(response != null)
            {
                _logger.LogInformation("No content");
                return StatusCode(400,response);
            }
            _orderService.AddProduct(productToCartDTO);
            _logger.LogInformation("Product added to cart successfully");
            return Ok("Product added to cart sucessfully");
        }

        ///<summary>
        /// Add new product to cart
        ///</summary>
        ///<return>Product added to cart successfully</return>
        [HttpPost]
        [Route("api/cart/create")]
        [AllowAnonymous]
        public void CreateCart([FromBody] string id)
        {
            _logger.LogInformation("Create created successfully");
            _orderService.CreateCart(Guid.Parse(id));
        }

        [HttpDelete]
        [Route("api/cart/product/{id}")]
        [Authorize(Roles = "User,Admin")]
        public IActionResult DeleteProductCart([FromRoute] Guid id)
        {
            _logger.LogInformation("Deleted product started");
            if (!ModelState.IsValid)
            {
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                _logger.LogError("Entered wrong data");
                return BadRequest(badRequest);
            }
            ErrorDTO isProductInCart = _orderService.IsProductInCart(id);
            if (isProductInCart != null)
            {
                _logger.LogError("Product not found in the cart");
                return NotFound(isProductInCart);
            }
            _logger.LogInformation("Product removed from cart successfully");
            return Ok("Product removed from Cart Successfully");
        }

        ///<summary>
        /// Updates qunatity in cart
        ///</summary>
        ///<return>Product Qunatity Updated Successfully</return>
        [HttpPut]
        [Route("api/cart")]
        [Authorize(Roles = "User,Admin")]
        public IActionResult UpdateQuantity([FromBody] UpdateCart updateCart)
        {
            _logger.LogInformation("Updating product quantity started");
            if (!ModelState.IsValid)
            {
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                _logger.LogError("Entered wrong data");
                return BadRequest(badRequest);
            }
            ErrorDTO updateProductQunatity = _orderService.UpdateProductQuantity(updateCart);
            if (updateProductQunatity != null)
            {
                _logger.LogError("Product not found");
                return StatusCode(404, updateProductQunatity);
            }
            _logger.LogInformation("Product Qunatity Updated Successfully");
            return Ok("Product Qunatity Updated Successfully");
        }

        ///<summary>
        /// Creates new wish list 
        ///</summary>
        ///<return>Guid</return>
        [HttpPost]
        [Route("api/wish-list")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult> CreateWishListProduct([FromBody] NewWishListProduct newWwishListProduct)
        {
            _logger.LogInformation("Create new wishlist started");
            if (!ModelState.IsValid)
            {
                _logger.LogError("Entered wrong data");
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO isTheProductExists = await _orderService.IsTheproductExists(newWwishListProduct.ProductId);
            if (isTheProductExists != null)
            {
                _logger.LogError("Product not found");
                return StatusCode(404, isTheProductExists);
            }
            ErrorDTO isWishListNameExists = _orderService.IsWishListNameExists(newWwishListProduct);
            if (isWishListNameExists != null)
            {
                _logger.LogError("Entered wrong data");
                return StatusCode(409, isWishListNameExists);
            }
            Guid id = _orderService.GetWishListId(newWwishListProduct.Name);
            _logger.LogInformation("New wishlist created");
            return StatusCode(201,id);
        }

        ///<summary>
        /// Delete user details
        ///</summary>
        [HttpDelete]
        [Route("api/user/{id}")]
        public void DeleteUser([FromRoute] Guid id)
        {
             _orderService.DeleteUserData(id);
            _logger.LogInformation("User data deleted");
        }

        ///<summary>
        /// Delete user wish list
        ///</summary>
        ///<return>Wish List deleted Successfully</return>
        [HttpDelete]
        [Route("api/wish-list/{id}")]
        [Authorize(Roles = "User,Admin")]
        public IActionResult DeleteWishList([FromRoute] Guid id)
        {
            _logger.LogInformation("Delete wishlist started");
            if (!ModelState.IsValid)
            {
                _logger.LogError("Entered wrong data");
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO deleteWishList = _orderService.DeleteWishList(id);
            if (deleteWishList != null)
            {
                _logger.LogError("Wish list not found");
                return StatusCode(404, deleteWishList);
            }
            _logger.LogInformation("Wish List deleted Successfully");
            return Ok("Wish List deleted Successfully");
        }

        ///<summary>
        /// Add Product to wish list
        ///</summary>
        ///<return>Product added to Wish list Successfully</return>
        [HttpPost]
        [Route("api/wish-list/product")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult> AddProductWishList([FromBody] WishListproduct wishListProduct)
        {
            _logger.LogInformation("Add product to wishlist strted");
            if (!ModelState.IsValid)
            {
                _logger.LogError("Entered wrong data");
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            try
            {
                ErrorDTO isTheProductExists = await _orderService.IsTheproductExists(wishListProduct.ProductId);
                if (isTheProductExists != null)
                {
                    _logger.LogError("Product not found ");
                    return StatusCode(404, isTheProductExists);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("Product service is unavailable");
                return StatusCode(500,ex.Message);
            }
            
            ErrorDTO isWishListExist = _orderService.CheckWishList(wishListProduct.WishListId);
            if (isWishListExist != null)
            {
                _logger.LogError("Wish list not found");
                return StatusCode(404, isWishListExist);
            }
            ErrorDTO saveProductWishList = _orderService.SaveProductWishList(wishListProduct);
            if (saveProductWishList != null)
            {
                _logger.LogError("Product already exist");
                return StatusCode(409, saveProductWishList);
            }
            _logger.LogInformation("Product added to Wish list Successfully");
            return Ok("Product added to Wish list Successfully");
        }

        ///<summary>
        /// Delete product in wishlist
        ///</summary>
        ///<return>Product removed from wish list successfully.</return>
        [HttpDelete]
        [Authorize(Roles = "Admin,User")]
        [Route("api/wish-list/{id}/product/{product-id}")]
        public IActionResult DeleteProductInWishList([FromRoute] Guid id, [FromRoute(Name = Constants.productId)] Guid productId)
        {
            _logger.LogInformation("Delete product in wish-list strted");
            if (!ModelState.IsValid)
            {
                _logger.LogError("Entered wrong data");
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO deleteProductWishList = _orderService.DeleteProductWishList(id, productId);
            if (deleteProductWishList != null)
            {
                _logger.LogError("Not found");
                return StatusCode(404, deleteProductWishList);
            }
            _logger.LogInformation("Product removed from wish list successfully.");
            return Ok("Product removed from wish list successfully.");
        }

        ///<summary>
        /// Move product from wish list to cart
        ///</summary>
        ///<return>Product moved from wish list to cart Successfully"</return>
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [Route("api/cart/wish-list")]
        public IActionResult MoveProductToCart([FromBody] WishListToCart wishListToCart)
        {
            _logger.LogInformation("Move product, wish list to cart strted");
            if (!ModelState.IsValid)
            {
                _logger.LogError("Entered wrong data");
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO isProductExistsInCart = _orderService.IsProductExistsInCart(wishListToCart);
            if (isProductExistsInCart != null)
            {
                _logger.LogError("Not found");
                return StatusCode(404, isProductExistsInCart);
            }
            _logger.LogInformation("Product moved from wish list to cart Successfully");
            return Ok("Product moved from wish list to cart Successfully");
           
        }

        ///<summary>
        /// Check out single products 
        ///</summary>
        ///<return>Check out single product done successfully</return>
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [Route("api/check-out/product")]
        public async Task<ActionResult> CheckOutProduct([FromBody] SingleProductCheckOutDTO singleProductCheckOutDTO)
        {
            _logger.LogInformation("Check out single product started");
            if (!ModelState.IsValid)
            {
                _logger.LogError("Entered wrong data");
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            try
            {
                ErrorDTO isProductExist = await _orderService.IsTheproductExists(singleProductCheckOutDTO.ProductId);
                if (isProductExist != null)
                {
                    _logger.LogError("Not found");
                    return StatusCode(404, isProductExist);
                }
                ErrorDTO response = await _orderService.IsPurchaseDetailsExist(singleProductCheckOutDTO);
                if(response != null)
                {
                    _logger.LogError("Not found");
                    return StatusCode(404, isProductExist);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Product service is unavailable");
                return StatusCode(500,ex.Message);
            }
            try
            {
                int response = await _orderService.CheckOut(singleProductCheckOutDTO);
                _logger.LogInformation("check out single product done successfully");
                return StatusCode(201,response);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("No Products in cart");
                return StatusCode(204, ex.Message);
            };
        }

        ///<summary>
        /// Check out all products in cart 
        ///</summary>
        ///<return>Check out cart done successfully</return>
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [Route("api/check-out/cart")]
        public async Task<ActionResult> CheckOutCart([FromBody] CheckOutCart checkOutCart)
        {
            _logger.LogInformation("Check out Cart started");
            if (!ModelState.IsValid)
            {
                _logger.LogError("Entered wrong data");
                ErrorDTO badRequest = _orderService.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            try
            {
                ErrorDTO response =await _orderService.IsPurchaseDetailsExist(checkOutCart);
                if (response != null)
                {
                    _logger.LogError("Not found");
                    return StatusCode(404, response);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("Product service is unavailable");
                return StatusCode(500,ex.Message);
            }
            try
            {
                ErrorDTO response = await _orderService.IsQunatityLeft(Guid.Empty,0);
                if (response != null)
                {
                    _logger.LogError("Not found");
                    return StatusCode(404, response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Product service is unavailable");
                StatusCode(500, ex.Message);
            }
            int? checkOutProduct =await _orderService.CheckOutCart(checkOutCart);           
            if(checkOutProduct == null)
            {
                _logger.LogInformation("Cart is empty");
                return StatusCode(204,"Cart is Empty");    
            }
            _logger.LogError("Check out cart done successfully");
            return StatusCode(201,checkOutProduct);
        }

        ///<summary>
        /// Get all products in cart 
        ///</summary>
        ///<return>Products in cart reterived successfully</return>
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        [Route("api/cart")]
        public async Task<ActionResult> GetProductsInCart()
        {
            _logger.LogInformation("Geting products in cart started");
            ErrorDTO isUserExist = _orderService.IsUserExist();
            if(isUserExist != null)
            {
                _logger.LogError("Not Found");
                return StatusCode(404,isUserExist);
            }
            try
            {
                List<ProductDTO> products = await _orderService.GetProductsInCart();
                if (products == null)
                {
                    _logger.LogInformation("Cart is empty");
                    return StatusCode(204, "Cart is Empty");
                }
                _logger.LogInformation("Products in cart reterived successfully");
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError("Product service is unavailable");
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        /// Get all products in wish list 
        ///</summary>
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        [Route("api/wish-list/{id}")]
        public async Task<ActionResult<List<WishListProductDTO>>> GetWishListProducts([FromRoute] Guid id)
        {
            _logger.LogInformation("Getting products in cart started");
            ErrorDTO isWishListExist = _orderService.IsWishListExist(id);
            if(isWishListExist != null)
            {
                _logger.LogError("Not Found");
                return StatusCode(404, isWishListExist);
            }
            //try
            //{
                List<WishListProductDTO> response = await _orderService.GetWishListProducts(id);
                if (response == null)
                {
                    _logger.LogInformation("Wish list empty");
                    return StatusCode(204, "Wish list empty");
                }
                _logger.LogInformation("All products in wishlist reterived successfully");
                return Ok(response);
           // }
            //catch(Exception ex)
            //{
                _logger.LogError("Product service is unavailable");
            return StatusCode(500);//,ex.Message);
            //}
            
        }
        ///<summary>
        /// Get all order details of user
        ///</summary>
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        [Route("api/bill")]
        public async Task<ActionResult> OrderDetails()
        {
            _logger.LogInformation("Getting order details started");
            ErrorDTO isUserExist = _orderService.IsUserExist();
            if (isUserExist != null)
            {
                _logger.LogError("Not Found ");
                return StatusCode(404, isUserExist);
            }

            List<OrderResponseDTO> orderDetails =await _orderService.GetOrderDetails();
            if(orderDetails == null)
            {
                _logger.LogInformation("No orders placed");
                return StatusCode(204,"No Orders placed");
            }
            _logger.LogInformation("Orders reterived successfully");
            return Ok(orderDetails);
        }

        ///<summary>
        /// Get  order details of using order id
        ///</summary>
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        [Route("api/bill/{id}")]
        public async Task<ActionResult> OrderDetail([FromRoute] int id)
        {
            _logger.LogInformation("Getting single order details started ");
            ErrorDTO isUserExist = _orderService.IsUserExist();
            if (isUserExist != null)
            {
                _logger.LogError("Not Found ");
                return StatusCode(404, isUserExist);
            }
            ErrorDTO isOrderIdExist = _orderService.IsOrderIdExist(id);
            if(isOrderIdExist != null)
            {
                _logger.LogError("Not Found ");
                return NotFound(isOrderIdExist);
            }
            OrderResponseDTO orderDetails =await _orderService.GetOrderDetails(id);
            _logger.LogInformation("All order details retrived successfully");
            return Ok(orderDetails);
        }
    }
}
