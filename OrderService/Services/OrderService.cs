
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
using Constants = Order_Service.Entities.Dtos.Constants;
using WishListProduct = Order_Service.Entity.Model.WishListProduct;

namespace Order_Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _context;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService( IHttpContextAccessor context,IOrderRepository orderRepository, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _orderRepository = orderRepository;
            IHostBuilder hostBuilder1 = Host.CreateDefaultBuilder()
                .ConfigureServices(Services =>
                {
                    Services.AddHttpClient(Constants.Product, config =>
            config.BaseAddress = new System.Uri(Constants.URL));
                });
            IHost host1 = hostBuilder1.Build();
            _httpClientFactory = host1.Services.GetRequiredService<IHttpClientFactory>();
        }
        ///<summary>
        /// checks the properties are valid or not 
        ///</summary>
        public ErrorDTO ModelStateInvalid(ModelStateDictionary ModelState)
        {
            return new ErrorDTO
            {
                type = ModelState.Keys.Select(src => src).FirstOrDefault(),
                statusCode = "400",
                message= ModelState.Values.Select(src => src.Errors[0].ErrorMessage).FirstOrDefault()

            };
        }
        ///<summary>
        /// Generates access token if the user is authorised
        ///</summary>
        public string AccessToken(string id)
        {
            JwtSecurityTokenHandler tokenhandler = new JwtSecurityTokenHandler();
            byte[] tokenKey = Encoding.UTF8.GetBytes(Constants.SecurityKey);

            SecurityTokenDescriptor tokenDeprictor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(
                new Claim[]
                {
                    new Claim(Constants.Role,Constants.User),
                     new Claim(Constants.Id,id)
                }
                ),
                Expires = DateTime.UtcNow.AddMinutes(90),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenhandler.CreateToken(tokenDeprictor);
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }
        ///<summary>
        /// checks products in the cart exist or not
        ///</summary>
        public async Task<ErrorDTO> CartProductsExist(List<ProductToCartDTO> productToCartDTOs)
        {
            string userId = _context.HttpContext.User.Claims.First(term => term.Type == Constants.Id).Value;
            Cart cart = _orderRepository.GetCartProducts(Guid.Parse(userId));
            if(cart == null)
            {
                return new ErrorDTO() {type="NoContent",message=$"Cart is Empty",statusCode="204" };
            }

            List<Guid> productIds = new List<Guid>();
            foreach (ProductToCartDTO item in productToCartDTOs)
            {
                productIds.Add(item.ProductId);
            }
            using HttpClient client = _httpClientFactory.CreateClient(Constants.Product);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ContentType));

            string accessToken = AccessToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.Bearer, accessToken);
            HttpResponseMessage response = client.PostAsync($"/api/cart/products", new StringContent(JsonConvert.SerializeObject(productIds),
                                  Encoding.UTF8, Constants.ContentType)).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Product service is unavailable");
            }
            string result1 = await response.Content.ReadAsStringAsync();
            Guid id;
            if (!Guid.TryParse(result1, out id))
            {
                return new ErrorDTO() { type = "NotFound", message = id + " Product id not found",statusCode="404" };
            }
            return null;
        }
        ///<summary>
        /// checks if the product exist or not 
        ///</summary>
        public async Task<ErrorDTO> IsTheproductExists(Guid id)
        {
            using HttpClient client = _httpClientFactory.CreateClient(Constants.Product);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ContentType));
            string userId = _context.HttpContext.User.Claims.First(src => src.Type == Constants.Id).Value;
            bool isUserExist = _orderRepository.IsTheUserExist(Guid.Parse(userId));
            if(!isUserExist)
            {
                return new ErrorDTO() {type="NotFound",message="User not found",statusCode="404" };
            }
            string accessToken = AccessToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.Bearer,accessToken);
            HttpResponseMessage response =  client.GetAsync($"/api/product/ocelot?id={id}").Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Product Service is unavailable");
            }
            string result =await response.Content.ReadAsStringAsync();

            string responseString = JsonConvert.DeserializeObject(result).ToString();
            QuantityResponse quantityResponse = JsonConvert.DeserializeObject<QuantityResponse>(responseString);
            if (quantityResponse.type == "product")
            {
                return new ErrorDTO { type = "InSufficent", message = quantityResponse.description,statusCode="404" };
            }
            return null;
        }
        ///<summary>
        /// adds product to user cart
        ///</summary>
        public void AddProduct(ProductToCartDTO productToCartDTO)
        {
            string userId = _context.HttpContext.User.Claims.First(src => src.Type == Constants.Id).Value;
            Product product = new Product()
            {
                Id = Guid.NewGuid(),
                ProductId= productToCartDTO.ProductId,
                Quantity= productToCartDTO.Quantity == 0 ? 1 : productToCartDTO.Quantity,
                IsActive = true,
                CreateBy = Guid.Parse(userId)
            };
            _orderRepository.AddProduct(product,Guid.Parse(userId));
        }
        ///<summary>
        /// checks if the product exist in the user cart
        ///</summary>
        public ErrorDTO IsProductInCart(Guid id)
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == Constants.Id).Value;
            bool isProductInCart = _orderRepository.IsProductInCart(id,Guid.Parse(userId));
            if(isProductInCart)
            {
                return null;
            }
            return new ErrorDTO {type="NotFound",message="Cart not contains Product with id",statusCode="404" };
        }
        ///<summary>
        /// updates product quantity in cart
        ///</summary>
        public ErrorDTO UpdateProductQuantity(UpdateCart updateCart)
        {
            string userId = _context.HttpContext.User.Claims.First(src => src.Type == Constants.Id).Value;
            bool isCartQuantityUpdated = _orderRepository.GetCart(updateCart,Guid.Parse(userId));
            if(!isCartQuantityUpdated)
            {
                return new ErrorDTO { type = "NotFound", message = "Product with id not found in Cart",statusCode="404" };
            }
            return null;
        }
        ///<summary>
        /// checks wish-list name already exist or not
        ///</summary>
        public ErrorDTO IsWishListNameExists(NewWishListProduct newWishListProduct)
        {
            string userId = _context.HttpContext.User.Claims.First(src => src.Type == Constants.Id).Value;
            Guid id = Guid.NewGuid();
            WishList wishList = new WishList()
            {
                Id = id,
                UserId = Guid.Parse(userId),
                Name = newWishListProduct.Name,
                WishListProduct = new List<WishListProduct>(),
                IsActive = true,
                CreatedDate = DateTime.Now,
                CreateBy = Guid.Parse(userId)
            };
            WishListProduct wishListProduct = new WishListProduct() 
            { Id = Guid.NewGuid(), ProductId = newWishListProduct.ProductId, WishListId = id,IsActive=true,CreateBy=Guid.Parse(userId),CreatedDate=DateTime.Now };
            wishList.WishListProduct.Add(wishListProduct);
            bool isWishListNameExists = _orderRepository.IsWishListNameExists(wishList);

            if(isWishListNameExists)
            {
                return null;
            }
            return new ErrorDTO {type="Conflict",message="Wish List name already exists",statusCode="409" };
        }
        ///<summary>
        /// Deletes wish-list 
        ///</summary>
        public ErrorDTO DeleteWishList(Guid id)
        {
            bool deleteWishList = _orderRepository.DeleteWishList(id);
            if(!deleteWishList)
            {
                return new ErrorDTO {type="NotFound",message="Wish list with id not found",statusCode="404" };
            }
            return null;
        }
        ///<summary>
        /// saves new wish-list 
        ///</summary>
        public ErrorDTO SaveProductWishList(WishListproduct wishListProduct)
        {
            string userId = _context.HttpContext.User.Claims.First(src => src.Type == Constants.Id).Value;
            Guid id = Guid.NewGuid();
            
            WishListProduct productWishlist = new WishListProduct() 
            { Id = Guid.NewGuid(),
                ProductId = wishListProduct.ProductId, WishListId = wishListProduct.WishListId 
                ,IsActive = true,
                CreatedDate = DateTime.Now,
                CreateBy = Guid.Parse(userId)
            };

            bool saveProduct = _orderRepository.SaveProductWishList(productWishlist);
            if(saveProduct)
            {
                return new ErrorDTO() {type="Conflilct",message="Product already added to wish list",statusCode="409" };
            }
            return null;  
        }
        ///<summary>
        /// checks wish-list name exist or not
        ///</summary>
        public ErrorDTO CheckWishList(Guid wishListId)
        {
            bool isWishListExist = _orderRepository.CheckWishList(wishListId);
            if(!isWishListExist)
            {
                return new ErrorDTO() {type="NotFound",message="Wish list id not found",statusCode="404" };
            }
            return null;
        }
        ///<summary>
        /// Deletes product in user wish-list
        ///</summary>
        public ErrorDTO DeleteProductWishList(Guid id, Guid productId)
        {
            Tuple<string,string> response = _orderRepository.DeleteProductWishList(id, productId);
            switch (response.Item1)
            {
                case ("wishlist"):
                    return new ErrorDTO { type = "NotFound", message = "Wish List with Id not found",statusCode="404" };
                case ("product"):
                    return new ErrorDTO { type = "NotFound", message = "Product with Id not found",statusCode="404" };
                default:
                    return null;
            }
        }
        ///<summary>
        /// Deletes product in cart if the product id doesnt exist return not found
        ///</summary>
        public ErrorDTO DeleteProductCart(Guid id)
        {
            string userId = _context.HttpContext.User.Claims.First(src => src.Type == Constants.Id).Value;
            bool isProductRemoved = _orderRepository.IsProductRemoved(id,Guid.Parse(userId));
            if(!isProductRemoved)
            {
                return new ErrorDTO() {type="NotFound",message="Product with id not found in the cart",statusCode="404" };
            }
            return null;
        }
        ///<summary>
        /// Checks if the product exist in the cart or not
        ///</summary>
        public ErrorDTO IsProductExistsInCart(WishListToCart cartTOWishList)
        {
            string userId = _context.HttpContext.User.Claims.First(src => src.Type == Constants.Id).Value;
            
            Tuple<string, string> response = _orderRepository.IsProductExistInCart(cartTOWishList,Guid.Parse(userId));
            switch (response.Item1)
            {
                case (Constants.Wishlist):
                    return new ErrorDTO { type = "NotFound", message = "Wish List with Id not found" , statusCode = "404" };
                case (Constants.Product):
                    return new ErrorDTO { type = "NotFound", message = "Product with Id not found",statusCode="404" };
                default:
                    return null;
            }
        }
        ///<summary>
        /// removes all products from cart and updates quantity in product service
        ///</summary>
        public async Task<int?> CheckOutCart(CheckOutCart checkOutCart)
        {
            string userId = _context.HttpContext.User.Claims.First(src => src.Type == Constants.Id).Value;
            Cart cart = _orderRepository.GetCartProducts(Guid.Parse(userId));
            if(cart.Product.Count() == 0)
            {
                return null;
            }
            using HttpClient client = _httpClientFactory.CreateClient(Constants.Product);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ContentType));
            string accessToken = AccessToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.Bearer, accessToken);

            HttpResponseMessage response = client.PostAsync($"/api/check-out", new StringContent(JsonConvert.SerializeObject(checkOutCart),
                        Encoding.UTF8, Constants.ContentType)).Result;
            string responseString = await response.Content.ReadAsStringAsync();
            string responseString1 = JsonConvert.DeserializeObject(responseString).ToString();
            CheckOutResponseDTO checkOutResponseDTO = JsonConvert.DeserializeObject<CheckOutResponseDTO>(responseString1);
            string stringResponse = JsonConvert.SerializeObject(checkOutResponseDTO.Address);
            List<ProductQuantity> products = new List<ProductQuantity>();
            foreach (Product item in cart.Product)
            {
                products.Add(new ProductQuantity() {Id=item.ProductId,Quantity=item.Quantity });
            }
            
            HttpResponseMessage response1 = client.PostAsync($"/api/cart/products/price", new StringContent(JsonConvert.SerializeObject(products),
                        Encoding.UTF8, Constants.ContentType)).Result;

            string result = await response1.Content.ReadAsStringAsync();
            string productResponse = JsonConvert.DeserializeObject(result).ToString();
            List<ProductPrice> productPrice = JsonConvert.DeserializeObject<List<ProductPrice>>(productResponse);
            
            float amount = 0;
            foreach (ProductPrice item in productPrice)
            {
                Product product = cart.Product.Where(find => find.ProductId == item.Id).First();
                float price = (float)product.Quantity * item.Price;
                amount = amount + price;
            }
            string paymentType = checkOutResponseDTO.Payment.ExpiryDate == null ? "UPI Payment" : "Debit Card";
            int billCount = _orderRepository.GetBillNo();
            Bill payment = new Bill()
            {
                CartId = cart.Id,
                OrderValue = amount,
                PaymentId = checkOutCart.PaymentId,
                AddressId=checkOutCart.AddressId,
                IsActive =true,
                CreatedDate = DateTime.Now,

            };
            cart.BillNo = billCount; 
            int billNo = _orderRepository.GenerateBillNo(cart, payment, cart.Id);
            CheckOutResponse checkOutResponse = new CheckOutResponse()
            {
                PaymentType = paymentType,
                OrderValue = amount,
                BillNo = billNo,
                ShippingAddress= checkOutCart.AddressId,

            };
            return billCount;
        }
        ///<summary>
        /// Checks quantity of the product in product service and return error 
        ///</summary>
        public async Task<ErrorDTO> IsQunatityLeft(Guid id, int quantity)
        {
            List<ProductQuantity> productQuantities = new List<ProductQuantity>();
            string userId = _context.HttpContext.User.Claims.First(src => src.Type == Constants.Id).Value;
            if (id == Guid.Empty)
            {
                Cart cart = _orderRepository.GetCartProducts(Guid.Parse(userId));

                foreach (Product item in cart.Product.Where(sel =>sel.IsActive))
                {
                    ProductQuantity productQuantity = new ProductQuantity()
                    {
                        Id = item.ProductId,
                        Quantity = item.Quantity
                    };
                    productQuantities.Add(productQuantity);
                }
            }
            else
            {
                ProductQuantity productQuantity = new ProductQuantity()
                {
                    Id = id,
                    Quantity = quantity
                };
                productQuantities.Add(productQuantity);
            }
            
            using HttpClient client = _httpClientFactory.CreateClient(Constants.Product);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ContentType));
           
            string accessToken = AccessToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.Bearer, accessToken);
            HttpResponseMessage response = client.PostAsync($"/api/cart/products/quantity", new StringContent(JsonConvert.SerializeObject(productQuantities),
                                  Encoding.UTF8, Constants.ContentType)).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Product Service is unavailable");
            }
            string result = await response.Content.ReadAsStringAsync();
            if (result != "")
            {
                string stringResponse = JsonConvert.DeserializeObject(result).ToString();
                ProductQuantity productQuantity = JsonConvert.DeserializeObject<ProductQuantity>(stringResponse);
                if(productQuantity.Quantity == -1)
                {
                    return new ErrorDTO() { type = "BadRequest", message = $"Product id {productQuantity.Id}, Out of stock {productQuantity.Quantity}",statusCode="400" };
                }
                if(productQuantity.Quantity == 0)
                {
                    return new ErrorDTO() { type = "BadRequest", message = $"Product id {productQuantity.Id}, Out of stock {productQuantity.Quantity}",statusCode="400" };
                }
                return new ErrorDTO() {type="BadRequest",message= $"Product id {productQuantity.Id}, left Quantity {productQuantity.Quantity}"};  
            }
            return null;

        }
        ///<summary>
        /// Checks card id exist or not
        ///</summary>
        public ErrorDTO IsCartIdExist(Guid id)
        {
            bool isCartExist = _orderRepository.IsCartExist(id);
            if(!isCartExist)
            {
                return new ErrorDTO() {type="NotFound",message ="Cart not found with the "+id,statusCode="400" };
            }
            return null;
        }
        ///<summary>
        /// Check-out single product and updates quantity in product service
        ///</summary>
        public async Task<int> CheckOut(SingleProductCheckOutDTO singleProductCheckOutDTO)
        {
            using HttpClient client = _httpClientFactory.CreateClient(Constants.Product);
            singleProductCheckOutDTO.Quantity = singleProductCheckOutDTO.Quantity == 0 ? 1 : singleProductCheckOutDTO.Quantity;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ContentType));
            Guid userId = Guid.Parse(_context.HttpContext.User.Claims.First(i => i.Type == Constants.Id).Value);

            string accessToken = AccessToken(userId.ToString());
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.Bearer, accessToken);
            HttpResponseMessage response = await client.GetAsync($"/api/product/ocelot?id={singleProductCheckOutDTO.ProductId}&categoryId={singleProductCheckOutDTO.CategoryId}");  
            string result = await response.Content.ReadAsStringAsync();

            string stringResponse = JsonConvert.DeserializeObject(result).ToString();
            QuantityResponse quantityResponse = JsonConvert.DeserializeObject<QuantityResponse>(stringResponse);
            if (singleProductCheckOutDTO.Quantity > int.Parse(quantityResponse.description))
            {
                throw new Exception("Product Quantity left "+ quantityResponse.description);
            }
            CheckOutCart checkOutCart = new CheckOutCart()
            {
                AddressId=singleProductCheckOutDTO.AddressId,
                PaymentId=singleProductCheckOutDTO.PaymentId
            };
            HttpResponseMessage response2 = client.PostAsync($"/api/check-out", new StringContent(JsonConvert.SerializeObject(checkOutCart),
                        Encoding.UTF8, Constants.ContentType)).Result;
            string result2 = await response2.Content.ReadAsStringAsync();
            string stringResponses = JsonConvert.DeserializeObject(result2).ToString();
            CheckOutResponseDTO checkOutResponseDTO = JsonConvert.DeserializeObject<CheckOutResponseDTO>(stringResponses);
            string addressString = JsonConvert.SerializeObject(checkOutResponseDTO.Address);
            string type = checkOutResponseDTO.Payment.ExpiryDate == null ? "UPI Payment" : "Card Payment";

            object data = JsonConvert.DeserializeObject(result);
            Guid id = Guid.NewGuid();
            int bill = _orderRepository.GetBillNo();
            Cart cart = new Cart()
            {
                Id = id,
                UserId =userId,
                BillNo = bill,
                Bill = new List<Bill>(),
                Product = new List<Product>(),
                CreateBy = userId,
                CreatedDate = DateTime.Now,
                IsActive=true

            };
            float value = (float)singleProductCheckOutDTO.Quantity * float.Parse(quantityResponse.type);
            Bill payment = new Bill()
            {
                OrderValue = value,
                CartId = id,
                PaymentId= singleProductCheckOutDTO.PaymentId,
                AddressId= singleProductCheckOutDTO.AddressId,
                CreateBy = userId,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            Product product = new Product()
            {
                Id = Guid.NewGuid(),
                ProductId = singleProductCheckOutDTO.ProductId,
                CartId = id,  
                Quantity = singleProductCheckOutDTO.Quantity,
                CreateBy = userId,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            cart.Bill.Add(payment);
            cart.Product.Add(product);
            
           
             _orderRepository.CheckOut(cart,(userId));
            ProductToCartDTO productToCartDTO = new ProductToCartDTO()
            {
                Quantity=singleProductCheckOutDTO.Quantity,
                ProductId=singleProductCheckOutDTO.ProductId,
               
            };
            HttpResponseMessage response1 =  client.PutAsync($"/api/product/update", new StringContent(JsonConvert.SerializeObject(productToCartDTO),
                                  Encoding.UTF8, Constants.ContentType)).Result;

            string result1 = await response.Content.ReadAsStringAsync();
            string stringResult = JsonConvert.DeserializeObject(result).ToString();
            QuantityResponse quantityResponse1 = JsonConvert.DeserializeObject<QuantityResponse>(stringResult);
            return bill;
        }
        ///<summary>
        /// Creates cart for user
        ///</summary>
        public void CreateCart(Guid id)
        {
            Cart cart = new Cart()
            {
                Id = Guid.NewGuid(),
                UserId = id,
                IsActive = true,
                CreatedDate = DateTime.Now,
                CreateBy= id
            };
            _orderRepository.CreateCart(cart);
        }
        ///<summary>
        /// retruns all products details in the user cart
        ///</summary>
        public async  Task<List<ProductDTO>> GetProductsInCart()
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == Constants.Id).Value;
            Cart cart = _orderRepository.GetAllProducstInCart(Guid.Parse(userId));
            
            List<ProductQuantity> productDetails = new List<ProductQuantity>();
            foreach (Product item in cart.Product.Where(sel =>sel.IsActive))
            {
                ProductQuantity product = new ProductQuantity()
                {
                    Id=item.ProductId,
                    Quantity=item.Quantity
                };
                productDetails.Add(product);
            }
            if (productDetails.Count() == 0)
            {
                return null;
            }
            using HttpClient client = _httpClientFactory.CreateClient(Constants.Product);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ContentType));

            string accessToken = AccessToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.Bearer, accessToken);
            HttpResponseMessage response =  client.PostAsync($"/api/product/details",new StringContent(JsonConvert.SerializeObject(productDetails),
                                  Encoding.UTF8, Constants.ContentType)).Result;
            if(!response.IsSuccessStatusCode)
            {
                throw new Exception("Product Service is unavailable");
            }
            string result = await response.Content.ReadAsStringAsync();

            string stringResponse = JsonConvert.DeserializeObject(result).ToString();
            List<ProductDTO> productList = JsonConvert.DeserializeObject<List<ProductDTO>>(stringResponse);
            foreach (ProductDTO item in productList)
            {
                foreach (Product each in cart.Product.Where(sel => sel.IsActive ))
                {
                    if(each.ProductId == item.Id)
                    {
                         if(int.Parse(item.Quantity) == 0)
                        {
                            item.Quantity = "Product out of stock";
                        }
                        else if (each.Quantity > int.Parse(item.Quantity))
                        {
                            item.Quantity = $"Quantity only left {each.Quantity}";
                        }
                        else
                        {
                            item.Quantity = each.Quantity.ToString();
                        }
                    }
                }
            }
            return productList;

        }
        ///<summary>
        /// Checks wish-list id exist or not
        ///</summary>
        public ErrorDTO IsWishListExist(Guid id)
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == Constants.Id).Value;
            ErrorDTO isUserExist = IsUserExist();
            if (isUserExist != null )
            {
                return new ErrorDTO() { type = "NotFound", message = "User not found",statusCode="404" };
            }
            List<WishListProduct> getProductsInWishList = _orderRepository.GetProductsInWishList(Guid.Parse(userId),id);
            if(getProductsInWishList == null)
            {
                return new ErrorDTO(){type="NotFound",message="WishList id not found" };
            }
            return null;
        }
        ///<summary>
        /// returns list or wish list products with user id
        ///</summary>
        public async  Task<List<WishListProductDTO>> GetWishListProducts(Guid id)
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == Constants.Id).Value;
            List<WishListProduct> getProductsInWishList = _orderRepository.GetProductsInWishList(Guid.Parse(userId), id);
            
            List<Guid> productDetails = new List<Guid>();
            foreach (WishListProduct item in getProductsInWishList.Where(sel => sel.IsActive))
            {
                productDetails.Add(item.ProductId);
            }
            if (productDetails.Count() == 0)
            {
                return null;
            }
            using HttpClient client = _httpClientFactory.CreateClient(Constants.Product);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ContentType));


            string accessToken = AccessToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.Bearer, accessToken);
            HttpResponseMessage response = client.PostAsync($"/api/wish-list/product/details", new StringContent(JsonConvert.SerializeObject(productDetails),
                                  Encoding.UTF8, Constants.ContentType)).Result;
            if(!response.IsSuccessStatusCode)
            {
                throw new Exception("Product service is unavailable");
            }

            string result = await response.Content.ReadAsStringAsync();

            string stringResponse = JsonConvert.DeserializeObject(result).ToString();
            List<ProductDTO> productListResponse = JsonConvert.DeserializeObject<List<ProductDTO>>(stringResponse);
            if(productListResponse.Count() == 0)
            {
                return null;
            }
            List<WishListProductDTO> productList = new List<WishListProductDTO>();
            foreach (ProductDTO item in productListResponse)
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
        ///<summary>
        /// returns created wish-list id 
        ///</summary>
        public Guid GetWishListId(string name)
        {
            Guid userId = Guid.Parse(_context.HttpContext.User.Claims.First(i => i.Type == Constants.Id).Value);
            return _orderRepository.GetWishlistId(name,userId);
        }
        ///<summary>
        /// Cheks if the purchase details exist in the user microservice
        ///</summary>
        public async Task<ErrorDTO> IsPurchaseDetailsExist(CheckOutCart checkOutCart)
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == Constants.Id).Value;
            bool IsCartIdExist = _orderRepository.IsCartExist(Guid.Parse(userId));    
            using HttpClient client = _httpClientFactory.CreateClient(Constants.Product);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ContentType));

            string accessToken = AccessToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.Bearer, accessToken);
            
            HttpResponseMessage response = client.PostAsync($"/api/check-out/details", new StringContent(JsonConvert.SerializeObject(checkOutCart),
                                  Encoding.UTF8, Constants.ContentType)).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("User service is unavailable");
            }

            string result = await response.Content.ReadAsStringAsync();
            if(result == "")
            {
                return null;
            }
            string stringResponse = JsonConvert.DeserializeObject(result).ToString();
            ErrorDTO errorResponse = JsonConvert.DeserializeObject<ErrorDTO>(stringResponse);
            switch (errorResponse.type)
            {
                case ("Address"):
                    return new ErrorDTO { type = "NotFound", message = "Address with Id not found",statusCode="404" };
                case ("Payment"):
                    return new ErrorDTO { type = "NotFound", message = "Payment with Id not found",statusCode="404" };
                default:
                    return null;
            }
        }
        ///<summary>
        /// Checks product details exist in the product microservice or not
        ///</summary>
        public async Task<ErrorDTO> IsPurchaseDetailsExist(SingleProductCheckOutDTO singleProductCheckOutDTO)
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == Constants.Id).Value;
            using HttpClient client = _httpClientFactory.CreateClient(Constants.Product);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ContentType));

            string accessToken = AccessToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.Bearer, accessToken);

            CheckOutCart checkOutCart = new CheckOutCart()
            {
                AddressId=singleProductCheckOutDTO.AddressId,
                PaymentId=singleProductCheckOutDTO.PaymentId
            };
            HttpResponseMessage response = client.PostAsync($"/api/check-out/details", new StringContent(JsonConvert.SerializeObject(checkOutCart),
                                  Encoding.UTF8, Constants.ContentType)).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("User service is unavailable");
            }

            string result = await response.Content.ReadAsStringAsync();
            if (result == "")
            {
                return null;
            }
            string stringResponse = JsonConvert.DeserializeObject(result).ToString();
            ErrorDTO errorResponse = JsonConvert.DeserializeObject<ErrorDTO>(stringResponse);
            switch (errorResponse.type)
            {
                case (Constants.Address):
                    return new ErrorDTO { type = "NotFound", message = errorResponse.message,statusCode="404" };
                case (Constants.Payment):
                    return new ErrorDTO { type = "NotFond", message = errorResponse.message,statusCode="404" };
                default:
                    return null;
            }
        }
        ///<summary>
        /// Deletes user account 
        ///</summary>
        public void DeleteUserData(Guid id)
        {
             _orderRepository.DeleteUserData(id);
        }
        ///<summary>
        /// Checks user exist or not
        ///</summary>
        public ErrorDTO IsUserExist()
        {
            string userId = _context.HttpContext.User.Claims.First(i => i.Type == Constants.Id).Value;
            bool isUserExist = _orderRepository.IsUserExist(Guid.Parse(userId));
            if(!isUserExist)
            {
                return new ErrorDTO() {type="NotFound",message="User not found",statusCode="404" };
            }
            return null;
        }
        ///<summary>
        /// retusn order details 
        ///</summary>
        public OrderResponseDTO GetOrderDetails(int billNo)
        {
            Guid userId = Guid.Parse(_context.HttpContext.User.Claims.First(i => i.Type == Constants.Id).Value);
            Bill listBill = _orderRepository.GetOrderDetails(userId, billNo);
            OrderResponseDTO orderResponseDTO = new OrderResponseDTO()
            {
                AddressId = listBill.AddressId,
                OrderValue = listBill.OrderValue,
                PaymentId = listBill.PaymentId,
                BillNo = listBill.Id
            };
            return orderResponseDTO;
        }
        ///<summary>
        /// returns all order details of the user
        ///</summary>
        public List<OrderResponseDTO> GetOrderDetails()
        {
            Guid userId = Guid.Parse(_context.HttpContext.User.Claims.First(i => i.Type == Constants.Id).Value);
            List<Bill> listBill = _orderRepository.GetOrderDetails(userId);
            if(listBill.Count() == 0)
            {
                return null;
            }
            List<OrderResponseDTO> orderResponseDTOs = new List<OrderResponseDTO>();
            foreach (Bill item in listBill)
            {
                OrderResponseDTO orderResponseDTO = new OrderResponseDTO()
                {
                    AddressId=item.AddressId,
                    OrderValue=item.OrderValue,
                    PaymentId=item.PaymentId,
                    BillNo=item.Id
                };
                orderResponseDTOs.Add(orderResponseDTO);
            }
            return orderResponseDTOs;
        }
        ///<summary>
        /// Checks order id exist or not
        ///</summary>
        public ErrorDTO IsOrderIdExist(int id)
        {
            bool isExist = _orderRepository.IsOrderIdExist(id);
            if(!isExist)
            {
                return new ErrorDTO() {type="NotFound",message="Bill id not found",statusCode="404" };
            }
            return null;
        }
    }
}
