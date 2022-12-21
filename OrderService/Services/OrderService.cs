using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Order_Service.Contracts;
using Order_Service.Entities.Dtos;
using Order_Service.Entity.DTo;
using Order_Service.Entity.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WishListProduct = Order_Service.Entity.Model.WishListProduct;

namespace Order_Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _context;
        private readonly IOrderRepository _orderRepository;

        public OrderService(IHttpClientFactory httpClientFactory, IHttpContextAccessor context,IOrderRepository orderRepository)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
            _orderRepository = orderRepository;
        }
        public ErrorDTO ModelStateInvalid(ModelStateDictionary ModelState)
        {
            return new ErrorDTO
            {
                type = ModelState.Keys.Select(src => src).FirstOrDefault(),
                description = ModelState.Values.Select(src => src.Errors[0].ErrorMessage).FirstOrDefault()
            };
        }
        public string AccessToken(string id)
        {
            JwtSecurityTokenHandler tokenhandler = new JwtSecurityTokenHandler();
            byte[] tokenKey = Encoding.UTF8.GetBytes("thisismySecureKey12345678");

            SecurityTokenDescriptor tokenDeprictor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(
                new Claim[]
                {
                           new Claim("role","User"),
                           new Claim("Id",id)
                }
                ),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenhandler.CreateToken(tokenDeprictor);
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }


        public async   Task<ErrorDTO> IsTheproductExists(Guid id, Guid categoryId)
        {
            using HttpClient client = _httpClientFactory.CreateClient("product");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
          //  HttpContent content = new StringContent(productId.ToString());
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;

            string accessToken = AccessToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",accessToken);
            HttpResponseMessage response = await client.GetAsync($"/api/product/ocelot?id={id}&categoryId={categoryId}");

            string result =await response.Content.ReadAsStringAsync();
            object data = JsonConvert.DeserializeObject(result);
            if (data == null)
            {
                return new ErrorDTO { type = "NotFound", description = "Product not found with the id" }; ;
            }
            if (data.ToString() == "False")
            {
                return new ErrorDTO { type = "NotFound", description = "Product not found with the id" };
            }
            
            return null;
        }
        public void AddProduct(ProductToCartDTO productToCartDTO)
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            Guid id = Guid.NewGuid();
            Cart cart = new Cart()
            {
                Id = id,
                UserId = Guid.Parse(userId),
                Product = new List<Product>(),
                BillNo = new List<Payment>(),
            };   
            Guid idd = Guid.NewGuid();
            Product product = new Product()
            { 
                Id = productToCartDTO.ProductId,
                CartId=id,
                ProductId= productToCartDTO.ProductId,
                Quantity= productToCartDTO.Quantity
            };

            cart.Product.Add(product);
            _orderRepository.AddProduct(cart);
        }

        public ErrorDTO IsProductInCart(Guid id)
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            bool isProductInCart = _orderRepository.IsProductInCart(id,Guid.Parse(userId));
            if(isProductInCart)
            {
                return null;
            }
            return new ErrorDTO {type="NotFound",description="Cart not contains Product with id" };
        }

        public ErrorDTO UpdateProductQuantity(UpdateCart updateCart)
        {
            
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            Cart cart = new Cart()
            {
                Id = updateCart.CartId,
                UserId = Guid.Parse(userId),
                Product = new List<Product>(),
                BillNo = new List<Payment>(),
            };
            Guid idd = Guid.NewGuid();
            Product product = new Product()
            {
                Id = updateCart.ProductId,
                CartId = updateCart.CartId,
                ProductId = updateCart.ProductId,
                Quantity = updateCart.Quantity
            };
            bool isProductInCart = _orderRepository.IsProductAdded(cart);
            if (!isProductInCart)
            {
                return new ErrorDTO {type="NotFound",description="Product with id not found in Cart" };
            }
            return null;
        }
        public ErrorDTO IsWishListNameExists(NewWishListProduct newWishListProduct)
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            Guid id = Guid.NewGuid();
            WishList wishList = new WishList()
            {
                Id = id,
                UserId = Guid.Parse(userId),
                Name = newWishListProduct.Name,
                WishListProduct = new List<WishListProduct>(),
            };
            WishListProduct wishListProduct = new WishListProduct() { Id = Guid.NewGuid(), ProductId = newWishListProduct.ProductId, WishListId = id };
            wishList.WishListProduct.Add(wishListProduct);
            bool isWishListNameExists = _orderRepository.IsWishListNameExists(wishList);
            if(isWishListNameExists)
            {
                return null;
            }
            return new ErrorDTO {type="Conflict",description="Wish List name already exists" };
        }
        public ErrorDTO DeleteWishList(Guid id)
        {
            bool deleteWishList = _orderRepository.DeleteWishList(id);
            if(!deleteWishList)
            {
                return new ErrorDTO {type="NotFound",description="Wish list with id not found" };
            }
            return null;
        }
        public ErrorDTO SaveProductWishList(WishListproduct wishListProduct)
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            Guid id = Guid.NewGuid();
            
            WishListProduct productWishlist = new WishListProduct() { Id = Guid.NewGuid(), ProductId = wishListProduct.ProductId, WishListId = id };

            Tuple<string, string> b = _orderRepository.SaveProductWishList(productWishlist);
            switch (b.Item1)
            {
                case ("wishlist"):
                    return new ErrorDTO {type=b.Item1,description="Wish List with Id not found" };
                case ("product"):
                    return new ErrorDTO {type=b.Item1,description="Product with Id not found" };
                default:
                    return null;
            }
            
        }
        public ErrorDTO DeleteProductWishList(Guid id, Guid productId)
        {
            Tuple<string,string> x = _orderRepository.DeleteProductWishList(id, productId);
            switch (x.Item1)
            {
                case ("wishlist"):
                    return new ErrorDTO { type = x.Item1, description = "Wish List with Id not found" };
                case ("product"):
                    return new ErrorDTO { type = x.Item1, description = "Product with Id not found" };
                default:
                    return null;
            }
        }
        public ErrorDTO DeleteProductCart(Guid id)
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            bool isProductRemoved = _orderRepository.IsProductRemoved(id,Guid.Parse(userId));
            if(!isProductRemoved)
            {
                return new ErrorDTO() {type="NotFound",description="Product with id not found in the cart" };
            }
            return null;
        }
        public ErrorDTO IsProductExistsInCart(CartToWishList cartTOWishList)
        {
            Tuple<string, string> x = _orderRepository.IsProductExistInCart(Cart,);
        }
    }
}
