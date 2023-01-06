using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Order_Service.Contracts;
using Order_Service.Entities.Dtos;
using Order_Service.Entity.DTo;
using Order_Service.Entity.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
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
                Expires = DateTime.UtcNow.AddMinutes(90),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenhandler.CreateToken(tokenDeprictor);
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }
        public async Task<ErrorDTO> CartProductsExist(List<ProductToCartDTO> productToCartDTOs)
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            var i = _orderRepository.GetCartProducts(Guid.Parse(userId));
            if(i == null)
            {
                return new ErrorDTO() {type="Cart",description=$"Cart is Empty" };
            }

            List<Guid> productIds = new List<Guid>();
            foreach (var item in productToCartDTOs)
            {
                productIds.Add(item.ProductId);
            }
            using HttpClient client = _httpClientFactory.CreateClient("product");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string accessToken = AccessToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = client.PostAsync($"/api/cart/products", new StringContent(JsonConvert.SerializeObject(productIds),
                                  Encoding.UTF8, "application/json")).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Product service is unavailable");
            }
            string result1 = await response.Content.ReadAsStringAsync();
            Guid id;
            if (!Guid.TryParse(result1, out id))
            {
                return new ErrorDTO() { type = "Product", description = id + " Product id not found" };
            }
            return null;
            

        }

        public async   Task<ErrorDTO> IsTheproductExists(Guid id,Guid categoryId)
        {
            using HttpClient client = _httpClientFactory.CreateClient("product");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            var t = _orderRepository.IsTheUserExist(Guid.Parse(userId));
            if(!t)
            {
                return new ErrorDTO() {type="User",description="User Account deleted" };
            }
            string accessToken = AccessToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",accessToken);
            HttpResponseMessage response = await client.GetAsync($"/api/product/ocelot?id={id}&categoryId={categoryId}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Product Service is unavailable");
            }
            string result =await response.Content.ReadAsStringAsync();

            var s = JsonConvert.DeserializeObject(result).ToString();
            var d = JsonConvert.DeserializeObject<QuantityResponse>(s);
            if(d.type == "category")
            {
                return new ErrorDTO() { type = "Category", description = categoryId.ToString() };
            }
            
            if (d.type == "product")
            {
                return new ErrorDTO { type = "Product", description =id.ToString()  }; 
            }
           
            return null;
        }
        public void AddProduct(ProductToCartDTO productToCartDTO)
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;

            Product product = new Product()
            {
                Id = Guid.NewGuid(),
                ProductId= productToCartDTO.ProductId,
                Quantity= productToCartDTO.Quantity,
                
            };
            _orderRepository.AddProduct(product,Guid.Parse(userId));
        }

        public ErrorDTO IsProductInCart(Guid id)
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            bool isProductInCart = _orderRepository.IsProductInCart(id,Guid.Parse(userId));
            if(isProductInCart)
            {
                return null;
            }
            return new ErrorDTO {type="Cart",description="Cart not contains Product with id" };
        }

        public ErrorDTO UpdateProductQuantity(UpdateCart updateCart)
        {
            
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            Cart? cart = _orderRepository.GetCart(Guid.Parse(userId));
            var x = cart.Product.Any(find =>find.ProductId == updateCart.ProductId);
            if (!x)
            {
                return new ErrorDTO { type = "Product", description = "Product with id not found in Cart" };
            }
            foreach (var item in cart.Product)
            {
                item.Quantity = updateCart.Quantity;
            }
           

            //Cart cart = new Cart()
            //{
            //    Id = updateCart.CartId,
            //    UserId = Guid.Parse(userId),
            //    Product = new List<Product>(),
               
            //};
            //Guid idd = Guid.NewGuid();
            //Product product = new Product()
            //{
                
            //    CartId = updateCart.CartId,
            //    ProductId = updateCart.ProductId,
            //    Quantity = updateCart.Quantity
            //};
           // cart.Product.Add(product);
             _orderRepository.UpdateCart(cart);
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
            return new ErrorDTO {type="Wish List",description="Wish List name already exists" };
        }
        public ErrorDTO DeleteWishList(Guid id)
        {
            bool deleteWishList = _orderRepository.DeleteWishList(id);
            if(!deleteWishList)
            {
                return new ErrorDTO {type="Wish List",description="Wish list with id not found" };
            }
            return null;
        }
        public ErrorDTO SaveProductWishList(WishListproduct wishListProduct)
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            Guid id = Guid.NewGuid();
            
            WishListProduct productWishlist = new WishListProduct() { Id = Guid.NewGuid(), ProductId = wishListProduct.ProductId, WishListId = wishListProduct.WishListId };

            bool b = _orderRepository.SaveProductWishList(productWishlist);
            if(b)
            {
                return new ErrorDTO() {type="Product",description="Product already added to wish list" };
            }
            return null;
            
        }
        public ErrorDTO CheckWishList(Guid wishListId)
        {
            var t = _orderRepository.CheckWishList(wishListId);
            if(!t)
            {
                return new ErrorDTO() {type="Wish List",description="Wish list id not found" };
            }
            return null;
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
                return new ErrorDTO() {type="Product",description="Product with id not found in the cart" };
            }
            return null;
        }
        public ErrorDTO IsProductExistsInCart(WishListToCart cartTOWishList)
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            
            Tuple<string, string> x = _orderRepository.IsProductExistInCart(cartTOWishList,Guid.Parse(userId));
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
        public async Task<CheckOutResponse> CheckOutCart(CheckOutCart checkOutCart)
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            Cart cart = _orderRepository.GetCartProducts(Guid.Parse(userId));
            if(cart.Product.Count() == 0)
            {
                return null;
            }
            using HttpClient client = _httpClientFactory.CreateClient("product");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string accessToken = AccessToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage response = client.PostAsync($"/api/check-out", new StringContent(JsonConvert.SerializeObject(checkOutCart),
                        Encoding.UTF8, "application/json")).Result;
            string result1 = await response.Content.ReadAsStringAsync();
            var s2 = JsonConvert.DeserializeObject(result1).ToString();
            var dd2 = JsonConvert.DeserializeObject<CheckOutResponseDTO>(s2);
            var add = JsonConvert.SerializeObject(dd2.Address);
            List<ProductQuantity> products = new List<ProductQuantity>();
            foreach (var item in cart.Product)
            {
                products.Add(new ProductQuantity() {Id=item.ProductId,Quantity=item.Quantity });
            }
            
            HttpResponseMessage response1 = client.PostAsync($"/api/cart/products/price", new StringContent(JsonConvert.SerializeObject(products),
                        Encoding.UTF8, "application/json")).Result;

            string result = await response1.Content.ReadAsStringAsync();
            var s1 = JsonConvert.DeserializeObject(result).ToString();
            var dd1 = JsonConvert.DeserializeObject<List<ProductPrice>>(s1);
            
            float amount = 0;
            foreach (var item in dd1)
            {
                var x = cart.Product.Where(find => find.ProductId == item.Id).First();
                float price = (float)x.Quantity * item.Price;
                amount = amount + price;
            }
            string paymentType = dd2.Payment.ExpiryDate == null ? "UPI Payment" : "Debit Card";
            Bill payment = new Bill()
            {
                OrderValue = amount,
                PaymentType = paymentType,
                Id = Guid.NewGuid(),
                ShippingAddress=dd2.Address.ToString()
            };
            int billNo = _orderRepository.GenerateBillNo(payment,cart.Id);
            CheckOutResponse checkOutResponse = new CheckOutResponse()
            {
                PaymentType = paymentType,
                OrderValue = amount,
                BillNo = billNo,
                ShippingAddress= add

            };
            return checkOutResponse;

        }
        public async Task<ErrorDTO> IsQunatityLeft()
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            Cart cart = _orderRepository.GetCartProducts(Guid.Parse(userId));
            List<ProductQuantity> productQuantities = new List<ProductQuantity>();
            foreach (var item in cart.Product)
            {
                ProductQuantity productQuantity = new ProductQuantity()
                {
                    Id=item.ProductId,
                    Quantity=item.Quantity
                };
                productQuantities.Add(productQuantity);
            }
            using HttpClient client = _httpClientFactory.CreateClient("product");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
           
            string accessToken = AccessToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = client.PostAsync($"/api/cart/products/quantity", new StringContent(JsonConvert.SerializeObject(productQuantities),
                                  Encoding.UTF8, "application/json")).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Product Service is unavailable");
            }
            string result = await response.Content.ReadAsStringAsync();
            if (result != "")
            {
                var s = JsonConvert.DeserializeObject(result).ToString();
                var dd = JsonConvert.DeserializeObject<ProductQuantity>(s);
                if(dd.Quantity == -1)
                {
                    return new ErrorDTO() { type = "Product", description = $"Product id {dd.Id}, Out of stock {dd.Quantity}" };
                }
                return new ErrorDTO() {type="Product",description= $"Product id {dd.Id}, left Quantity {dd.Quantity}"};
               
            }

            
            return null;

        }
        public ErrorDTO isCartIdExist(Guid id)
        {
            var t= _orderRepository.IsCartExist(id);
            if(!t)
            {
                return new ErrorDTO() {type="Cart",description="Cart not found with the "+id };
            }
            return null;
        }

        public async Task<CheckOutResponse> CheckOut(SingleProductCheckOutDTO singleProductCheckOutDTO)
        {
            using HttpClient client = _httpClientFactory.CreateClient("product");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;

            string accessToken = AccessToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await client.GetAsync($"/api/product/ocelot?id={singleProductCheckOutDTO.ProductId}&categoryId={singleProductCheckOutDTO.CategoryId}");  
            string result = await response.Content.ReadAsStringAsync();

            var s = JsonConvert.DeserializeObject(result).ToString();
            var dd = JsonConvert.DeserializeObject<QuantityResponse>(s);
            if (singleProductCheckOutDTO.Quantity > int.Parse(dd.description))
            {
                throw new Exception("Product Quantity left "+dd.description);
            }
            CheckOutCart checkOutCart = new CheckOutCart()
            {
                AddressId=singleProductCheckOutDTO.AddressId,
                PaymentId=singleProductCheckOutDTO.PaymentId
            };
            HttpResponseMessage response2 = client.PostAsync($"/api/check-out", new StringContent(JsonConvert.SerializeObject(checkOutCart),
                        Encoding.UTF8, "application/json")).Result;
            string result2 = await response2.Content.ReadAsStringAsync();
            var s2 = JsonConvert.DeserializeObject(result2).ToString();
            var dd2 = JsonConvert.DeserializeObject<CheckOutResponseDTO>(s2);
            var add = JsonConvert.SerializeObject(dd2.Address);
            string type = dd2.Payment.ExpiryDate == null ? "UPI Payment" : "Card Payment";

            object data = JsonConvert.DeserializeObject(result);
            Cart cart = new Cart()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse(userId),
                BillNo = 1,
                Bill = new List<Bill>(),
                Product = new List<Product>(),
            };
            var value = (float)singleProductCheckOutDTO.Quantity * float.Parse(dd.type);
            Bill payment = new Bill()
            {
                Id = Guid.NewGuid(),
                OrderValue = value,
                PaymentType= type,
                BillNo = 1,
                ShippingAddress= add
            };
            Product product = new Product()
            { 
                Id = Guid.NewGuid(),
                ProductId = singleProductCheckOutDTO.ProductId,
                CartId = Guid.NewGuid(),
                Quantity = singleProductCheckOutDTO.Quantity
            };
            cart.Bill.Add(payment);
            cart.Product.Add(product);
            var v = _orderRepository.GetBillNo();
            cart.BillNo = v;
             _orderRepository.CheckOut(cart, Guid.Parse(userId));
            ProductToCartDTO productToCartDTO = new ProductToCartDTO()
            {
                Quantity=singleProductCheckOutDTO.Quantity,
                ProductId=singleProductCheckOutDTO.ProductId,
                CategoryId=singleProductCheckOutDTO.CategoryId
            };
            HttpResponseMessage response1 =  client.PutAsync($"/api/product/update", new StringContent(JsonConvert.SerializeObject(productToCartDTO),
                                  Encoding.UTF8, "application/json")).Result;

            string result1 = await response.Content.ReadAsStringAsync();
            if (!response1.IsSuccessStatusCode)
            {
                throw new Exception();
            }
            var s1 = JsonConvert.DeserializeObject(result).ToString();
            var dd1 = JsonConvert.DeserializeObject<QuantityResponse>(s);
            return new CheckOutResponse()
            {
                BillNo= v,
                OrderValue=value,
                PaymentType= type,
                ShippingAddress= add
            };
        }
        public void CreateCart(Guid id)
        {
            Cart cart = new Cart()
            {
                Id = Guid.NewGuid(),
                UserId = id,
            };
            _orderRepository.CreateCart(cart);
        }
        public async  Task<List<ProductDTO>> GetProductsInCart()
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            var t = _orderRepository.GetAllProducstInCart(Guid.Parse(userId));
            if(t.Product.Count() == 0)
            {
                return null;
            }
            List<ProductQuantity> productDetails = new List<ProductQuantity>();
            foreach (var item in t.Product)
            {
                ProductQuantity product = new ProductQuantity()
                {
                    Id=item.ProductId,
                    Quantity=item.Quantity
                };
                productDetails.Add(product);
            }
            using HttpClient client = _httpClientFactory.CreateClient("product");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string accessToken = AccessToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response =  client.PostAsync($"/api/product/details",new StringContent(JsonConvert.SerializeObject(productDetails),
                                  Encoding.UTF8, "application/json")).Result;
            if(!response.IsSuccessStatusCode)
            {
                throw new Exception("Product Service is unavailable");
            }
            string result = await response.Content.ReadAsStringAsync();

            var s = JsonConvert.DeserializeObject(result).ToString();
            var dd = JsonConvert.DeserializeObject<List<ProductDTO>>(s);
            foreach (var item in dd)
            {
                foreach (var each in t.Product)
                {
                    if(each.ProductId == item.Id)
                    {
                        if(each.Quantity > int.Parse(item.Quantity))
                        {
                            item.Quantity = $"Quantity only left {each.Quantity}";
                        }
                        if(int.Parse(item.Quantity) == 0)
                        {
                            item.Quantity = "Product out of stock";
                        }
                        item.Quantity = each.Quantity.ToString();
                    }
                }
            }
            return dd;

        }
        public ErrorDTO IsWishListExist(Guid id)
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            var x = IsUserExist();
            if (x != null )
            {
                return new ErrorDTO() { type = "User", description = "User Account deleted" };
            }
            var getProductsInWishList = _orderRepository.GetProductsInWishList(Guid.Parse(userId),id);
            if(getProductsInWishList == null)
            {
                return new ErrorDTO(){type="Wish List",description="WishList id not found" };
            }
            return null;
        }
        public async  Task<List<WishListProductDTO>> GetWishListProducts(Guid id)
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            var getProductsInWishList = _orderRepository.GetProductsInWishList(Guid.Parse(userId), id);
            if(getProductsInWishList == null)
            {
                return null;
            }

            List<Guid> productDetails = new List<Guid>();
            foreach (var item in getProductsInWishList)
            {
                productDetails.Add(item.ProductId);
            }
            using HttpClient client = _httpClientFactory.CreateClient("product");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            string accessToken = AccessToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = client.PostAsync($"/api/wish-list/product/details", new StringContent(JsonConvert.SerializeObject(productDetails),
                                  Encoding.UTF8, "application/json")).Result;
            if(!response.IsSuccessStatusCode)
            {
                throw new Exception("Product service is unavailable");
            }

            string result = await response.Content.ReadAsStringAsync();

            var s = JsonConvert.DeserializeObject(result).ToString();
            var dd = JsonConvert.DeserializeObject<List<ProductDTO>>(s);
            if(dd.Count() == 0)
            {
                return null;
            }
            List<WishListProductDTO> productList = new List<WishListProductDTO>();
            foreach (var item in dd)
            {
                WishListProductDTO wishListProductDTO = new WishListProductDTO();
                if (item.Name == null)
                {
                    wishListProductDTO.Id = item.Id;
                }
                else if(int.Parse(item.Quantity) == 0)
                {
                    wishListProductDTO.Id = item.Id;
                    wishListProductDTO.Name = item.Name;
                    wishListProductDTO.Description = item.Description;
                    wishListProductDTO.Price = item.Price;
                    wishListProductDTO.Qunatity = "Product is out of stock";
                    wishListProductDTO.Asset = item.Asset;
                }
                else
                {
                    wishListProductDTO.Id = item.Id;
                    wishListProductDTO.Name = item.Name;
                    wishListProductDTO.Description = item.Description;
                    wishListProductDTO.Price = item.Price;
                    wishListProductDTO.Qunatity = item.Price.ToString();
                    wishListProductDTO.Asset = item.Asset;
                }
                productList.Add(wishListProductDTO);
            }
            return productList;
        }
        public Guid GetWishListId(string name)
        {
            return _orderRepository.GetWishlistId(name);
        }

        public async Task<ErrorDTO> IsPurchaseDetailsExist(CheckOutCart checkOutCart)
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            var g = _orderRepository.IsCartExist(Guid.Parse(userId));    
            using HttpClient client = _httpClientFactory.CreateClient("product");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string accessToken = AccessToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
            HttpResponseMessage response = client.PostAsync($"/api/check-out/details", new StringContent(JsonConvert.SerializeObject(checkOutCart),
                                  Encoding.UTF8, "application/json")).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("User service is unavailable");
            }

            string result = await response.Content.ReadAsStringAsync();
            if(result == "")
            {
                return null;
            }
            var s = JsonConvert.DeserializeObject(result).ToString();
            var dd = JsonConvert.DeserializeObject<ErrorDTO>(s);
            switch (dd.type)
            {
                case ("Address"):
                    return new ErrorDTO { type = dd.type, description = "Address with Id not found" };
                case ("Payment"):
                    return new ErrorDTO { type = dd.type, description = "Payment with Id not found" };
                default:
                    return null;
            }
        }
        public async Task<ErrorDTO> IsPurchaseDetailsExist(SingleProductCheckOutDTO singleProductCheckOutDTO)
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            using HttpClient client = _httpClientFactory.CreateClient("product");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string accessToken = AccessToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            CheckOutCart checkOutCart = new CheckOutCart()
            {
                AddressId=singleProductCheckOutDTO.AddressId,
                PaymentId=singleProductCheckOutDTO.PaymentId
            };
            HttpResponseMessage response = client.PostAsync($"/api/check-out/details", new StringContent(JsonConvert.SerializeObject(checkOutCart),
                                  Encoding.UTF8, "application/json")).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("User service is unavailable");
            }

            string result = await response.Content.ReadAsStringAsync();
            if (result == "")
            {
                return null;
            }
            var s = JsonConvert.DeserializeObject(result).ToString();
            var dd = JsonConvert.DeserializeObject<ErrorDTO>(s);
            switch (dd.type)
            {
                case ("Address"):
                    return new ErrorDTO { type = dd.type, description = dd.description };
                case ("Payment"):
                    return new ErrorDTO { type = dd.type, description = dd.description };
                default:
                    return null;
            }
        }
        public void DeleteUserData(Guid id)
        {
             _orderRepository.DeleteUserData(id);
        }
        public ErrorDTO IsUserExist()
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            var t = _orderRepository.IsUserExist(Guid.Parse(userId));
            if(!t)
            {
                return new ErrorDTO() {type="User",description="User Account deleted" };
            }
            return null;
        }
    }
}
