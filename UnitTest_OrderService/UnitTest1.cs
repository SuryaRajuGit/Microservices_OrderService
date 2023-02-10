using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Order_Service.Controllers;
using Order_Service.Entities.Dtos;
using Order_Service.Entity.Model;
using Order_Service.Mappers;
using Order_Service.Repository;
using Order_Service.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Xunit;
using Mapper = Order_Service.Mappers.Mapper;

namespace UnitTest_OrderService
{
    public class UnitTest1
    {
        
        private readonly IMapper _mapper;
        private readonly OrderController _userController;
        private readonly ILogger _logger;
        private OrderContext _context;
        private OrderRepository _repository;
        private OrderService _service;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;

        public HttpClient CreateClient(string name)
        {
            return _httpClientFactory.CreateClient(name);
        }

        public UnitTest1()
        {

            IHostBuilder hostBuilder = Host.CreateDefaultBuilder().
            ConfigureLogging((builderContext, loggingBuilder) =>
            {
                loggingBuilder.AddConsole((options) =>
                {
                    options.IncludeScopes = true; 
                });
            });
            IHost host = hostBuilder.Build();
            ILogger<OrderController> _logger = host.Services.GetRequiredService<ILogger<OrderController>>();

            MapperConfiguration mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new Mapper());
            });


  
            Claim claim = new Claim("role", "User");
            Claim claim1 = new Claim("Id", "8d0c1df7-a887-4453-8af3-799e4a7ed013");
            ClaimsIdentity identity = new ClaimsIdentity(new[] { claim, claim1 }, "BasicAuthentication");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            GenericIdentity identityy = new GenericIdentity("some name", "test");
            ClaimsPrincipal contextUser = new ClaimsPrincipal(identity); 


            DefaultHttpContext httpContext = new DefaultHttpContext()
            {
                User = contextUser
            };

            HttpContextAccessor _httpContextAccessor = new HttpContextAccessor()
            {
                HttpContext = httpContext
            };


            IMapper mapper = mappingConfig.CreateMapper();
            _mapper = mapper;
            DbContextOptions<OrderContext> options = new DbContextOptionsBuilder<OrderContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _context = new OrderContext(options);

            _repository = new OrderRepository(_context);

            _service = new OrderService(_httpContextAccessor, _repository,  mapper);

            _userController = new OrderController(_service, _logger);


            AddData();
            _context.Database.EnsureCreated();
        }
        public void AddData()
        {
            string path = @"..\..\..\..\..\OrderService\OrderService\Entities\UnitTest_Files\TextFile.csv";
            string ReadCSV = File.ReadAllText(path);
            string[] data = ReadCSV.Split('\r');
            foreach (string item in data)
            {
                string[] row = item.Split(",");
                
                Cart cart = new Cart()
                {
                    Id = Guid.Parse(row[0]),
                    UserId = Guid.Parse(row[1]),
                    BillNo = int.Parse(row[2]),
                    Product = new List<Product>(),
                    Bill = new List<Bill>(),
                    IsActive=true
                };
                if(row[2] == "1")
                {
                    Bill bill = new Bill()
                    {
                        Id = int.Parse(row[2]),
                        OrderValue = int.Parse(row[3]),
                        PaymentId = Guid.Parse(row[4]),
                        AddressId = Guid.Parse(row[5]),
                        CartId = Guid.Parse(row[0]),
                        IsActive=true
                    };
                    cart.Bill.Add(bill);
                }
                WishList wishList = new WishList()
                {
                    Id = Guid.Parse(row[9]),
                    Name = row[10],
                    UserId = Guid.Parse(row[1]),
                    WishListProduct = new List<WishListProduct>(),
                    IsActive=true
                };
                WishListProduct wishListProduct = new WishListProduct()
                {
                    Id = Guid.Parse(row[11]),
                    ProductId = Guid.Parse(row[8]),
                    WishListId=Guid.Parse(row[9]) ,
                    IsActive=true
                };
                wishList.WishListProduct.Add(wishListProduct);
                Product product = new Product()
                {
                    Id = Guid.Parse(row[6]),
                    ProductId = Guid.Parse(row[8]),
                    Quantity = int.Parse(row[7]),
                    CartId= Guid.Parse(row[0]),
                    IsActive=true
                };
                
                cart.Product.Add(product);
                _context.WishList.Add(wishList);
                _context.Add(cart);
            }
            _context.SaveChanges();
        }

        [Fact]
        public async Task AddProductCart_Test()
        {
            ProductToCartDTO productToCartDTO = new ProductToCartDTO()
            {
                Quantity = 1,
                ProductId = Guid.Parse("4FA3D2FA-FEA1-4A86-A8CF-E8B4817D35FC"),
            };
            ProductToCartDTO productToCartDTO1 = new ProductToCartDTO()
            {
                Quantity = 1,
                ProductId = Guid.Parse("4FA3D2FA-FEA1-4A86-A8CF-E8B4817D35FE"),
            };
            ActionResult response =await  _userController.AddProductCart(productToCartDTO);
            ActionResult response1 =await  _userController.AddProductCart(productToCartDTO1);

            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            ObjectResult result1= Assert.IsType<ObjectResult>(response1);

            Assert.Equal(200, result.StatusCode);
            Assert.Equal(404, result1.StatusCode);
        }
        [Fact]
        public void CreateCart_Test()
        {
            string id = "8d0c1df7-a887-4453-8af3-799e4a7ed013";
            _userController.CreateCart(id);
            Assert.True(true);

        }
        [Fact]
        public void UpdateQuantity_Test()
        {
            UpdateCart updateCart = new UpdateCart()
            {
                ProductId = Guid.Parse("4fa3d2fa-fea1-4a86-a8cf-e8b4817d35fc"),
                Quantity = 1
            };
            UpdateCart updateCart1 = new UpdateCart()
            {
                ProductId = Guid.Parse("B08AD56A-46E8-40B4-88A9-1A2CE2DFF94B"),
                Quantity = 1
            };
            IActionResult response = _userController.UpdateQuantity(updateCart);
            IActionResult response1 = _userController.UpdateQuantity(updateCart1);

            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            ObjectResult result1 = Assert.IsType<ObjectResult>(response1);

            Assert.Equal(200, result.StatusCode);
            Assert.Equal(404, result1.StatusCode);
        }
        [Fact]
        public void CreateWishListProduct_Test()
        {
            NewWishListProduct newWishListProduct = new NewWishListProduct()
            {
                ProductId = Guid.Parse("C4EC41BB-A772-4909-8AEE-C3010F423132"),
                Name = "Wish_1",
            };
            NewWishListProduct newWishListProduct1 = new NewWishListProduct()
            {
                ProductId = Guid.Parse("4FA3D2FA-FEA1-4A86-A8CF-E8B4817D35FC"),
                Name = "Wish_1",
            };
            ActionResult response = _userController.CreateWishListProduct(newWishListProduct).Result;
            ActionResult response1 = _userController.CreateWishListProduct(newWishListProduct1).Result;
            ActionResult response2 = _userController.CreateWishListProduct(newWishListProduct1).Result;

            ObjectResult result = Assert.IsType<ObjectResult>(response);
            ObjectResult result1 = Assert.IsType<ObjectResult>(response1);
            ObjectResult result2 = Assert.IsType<ObjectResult>(response2);

            Assert.Equal(404, result.StatusCode);
            Assert.Equal(201, result1.StatusCode);
            Assert.Equal(409, result2.StatusCode);
        }
        [Fact]
        public void DeleteUser_Test()
        {
            _userController.DeleteUser(Guid.Parse("8d0c1df7-a887-4453-8af3-799e4a7ed013"));
            Assert.True(true);
        }
        [Fact]
        public void DeleteWishList_Test()
        {
            IActionResult response = _userController.DeleteWishList(Guid.Parse("747f9ed2-47ff-4467-b635-b6079c935f88"));
            IActionResult response1 = _userController.DeleteWishList(Guid.Parse("747f9ed2-47ff-4467-b635-b6079c935f89"));
            
            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            ObjectResult result1 = Assert.IsType<ObjectResult>(response1);
            
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(404, result1.StatusCode);
        }
        [Fact]
        public void AddProductWishList_Test()
        {
            WishListproduct wishListproduct = new WishListproduct()
            {
                WishListId = Guid.Parse("747f9ed2-47ff-4467-b635-b6079c935f88"),
                ProductId = Guid.Parse("4FA3D2FA-FEA1-4A86-A8CF-E8B4817D35FC"),
                CategoryId = Guid.Parse("4944226F-36A7-445F-A9E5-D5C2BA1F525F")
            };
            WishListproduct wishListproduct1 = new WishListproduct()
            {
                WishListId = Guid.Parse("747f9ed2-47ff-4467-b635-b6079c935f88"),
                ProductId = Guid.Parse("2a52169a-e58f-42e8-bc0e-4603c361c589"),
                CategoryId = Guid.Parse("4944226F-36A7-445F-A9E5-D5C2BA1F525F")
            };
            WishListproduct wishListproduct2 = new WishListproduct()
            {
                WishListId = Guid.Parse("747f9ed2-47ff-4467-b635-b6079c935f89"),
                ProductId = Guid.Parse("875DF98F-F0D1-406C-BA0E-AD76848E73C5"),
                CategoryId = Guid.Parse("4944226F-36A7-445F-A9E5-D5C2BA1F525F")
            };
            ActionResult response = _userController.AddProductWishList(wishListproduct).Result;
            ActionResult response1 = _userController.AddProductWishList(wishListproduct1).Result;
            ActionResult response2 = _userController.AddProductWishList(wishListproduct2).Result;


            ObjectResult result = Assert.IsType<ObjectResult>(response);
            OkObjectResult result1 = Assert.IsType<OkObjectResult>(response1);
            ObjectResult result2 = Assert.IsType<ObjectResult>(response2);

            Assert.Equal(200, result1.StatusCode);
            Assert.Equal(409, result.StatusCode);
            Assert.Equal(404, result2.StatusCode);
        }
        [Fact]
        public void DeleteProductInWishList_Test()
        {
            IActionResult response = _userController.DeleteProductInWishList(Guid.Parse("747f9ed2-47ff-4467-b635-b6079c935f88"),
                Guid.Parse("4fa3d2fa-fea1-4a86-a8cf-e8b4817d35fc"));
            IActionResult response1 = _userController.DeleteProductInWishList(Guid.Parse("747f9ed2-47ff-4467-b635-b6079c935f80"),
               Guid.Parse("b08ad56a-46e8-40b4-88a9-1a2ce2dff94a"));

            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            ObjectResult result1 = Assert.IsType<ObjectResult>(response1);

            Assert.Equal(200, result.StatusCode);
            Assert.Equal(404, result1.StatusCode);
        }
        [Fact]
        public void GetProductsInCart_Test()
        {
            ActionResult response = _userController.GetProductsInCart().Result;

            OkObjectResult result = Assert.IsType<OkObjectResult>(response);

            Assert.Equal(200, result.StatusCode);
        }
        [Fact]
        public void GetWishListProducts_Test()
        {
            Guid id = Guid.Parse("747f9ed2-47ff-4467-b635-b6079c935f88");
            Guid id1 = Guid.Parse("747f9ed2-47ff-4467-b635-b6079c935f89");

            ActionResult<List<WishListProductDTO>> response  = _userController.GetWishListProducts(id).Result;
            ActionResult<List<WishListProductDTO>> response1  = _userController.GetWishListProducts(id1).Result;

            OkObjectResult result = Assert.IsType<OkObjectResult>(response.Result);
            ObjectResult result1 = Assert.IsType<ObjectResult>(response1.Result);
            
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(404, result1.StatusCode);
        }
        [Fact]
        public void OrderDetails_Test()
        {
            ActionResult response = _userController.GetProductsInCart().Result;

            OkObjectResult result = Assert.IsType<OkObjectResult>(response);

            Assert.Equal(200, result.StatusCode);
        }
        [Fact]
        public async Task  OrderDetail_Test()
        {
            IActionResult response =await _userController.OrderDetail(1);
            IActionResult response1 =await _userController.OrderDetail(100);

            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            NotFoundObjectResult result1 = Assert.IsType<NotFoundObjectResult>(response1);

            Assert.Equal(200, result.StatusCode);
            Assert.Equal(404, result1.StatusCode);
        }
        [Fact]
        public void DeleteProductCart_Test()
        {
            IActionResult response = _userController.DeleteProductCart(Guid.Parse("4fa3d2fa-fea1-4a86-a8cf-e8b4817d35fc"));
            IActionResult response1 = _userController.DeleteProductCart(Guid.Parse("b08ad56a-46e8-40b4-88a9-1a2ce2dff94b"));

            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            NotFoundObjectResult result1 = Assert.IsType<NotFoundObjectResult>(response1);

            Assert.Equal(200, result.StatusCode);
            Assert.Equal(404, result1.StatusCode);
        }
        [Fact]
        public void MoveProductToCart_Test()
        {
            WishListToCart cart = new WishListToCart()
            {
                ProductId = "4fa3d2fa-fea1-4a86-a8cf-e8b4817d35fc",
                WishListId = "747f9ed2-47ff-4467-b635-b6079c935f88"
            };
            WishListToCart cart1 = new WishListToCart()
            {
                ProductId = "b08ad56a-46e8-40b4-88a9-1a2ce2dff94b",
                WishListId = "747f9ed2-47ff-4467-b635-b6079c935f88"
            };
            IActionResult response = _userController.MoveProductToCart(cart);
            IActionResult response1 = _userController.MoveProductToCart(cart1);

            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            ObjectResult result1 = Assert.IsType<ObjectResult>(response1);

            Assert.Equal(200, result.StatusCode);
            Assert.Equal(404, result1.StatusCode);
        }
        [Fact]
        public async Task CheckOutProduct_Test()
        {

            SingleProductCheckOutDTO singleProductCheckOutDTO1 = new SingleProductCheckOutDTO()
            {
                AddressId = Guid.Parse("DF16FC14-296F-4E48-910D-AFA7BA1E747F"),
                PaymentId = Guid.Parse("377D66FB-EACF-4637-9CC2-069AEF71FB4B"),
                ProductId = Guid.Parse("4FA3D2FA-FEA1-4A86-A8CF-E8B4817D35FC"),
                Quantity = 1,
                CategoryId = Guid.Parse("4944226F-36A7-445F-A9E5-D5C2BA1F525F")
            };
            SingleProductCheckOutDTO singleProductCheckOutDTO = new SingleProductCheckOutDTO()
            {
                AddressId = Guid.Parse("DF16FC14-296F-4E48-910D-AFA7BA1E747E"),
                PaymentId = Guid.Parse("377D66FB-EACF-4637-9CC2-069AEF71FB4B"),
                ProductId = Guid.Parse("4FA3D2FA-FEA1-4A86-A8CF-E8B4817D35FC"),
                Quantity = 1,
                CategoryId = Guid.Parse("4944226F-36A7-445F-A9E5-D5C2BA1F525F")
            };

            ActionResult response =await  _userController.CheckOutProduct(singleProductCheckOutDTO);
            ActionResult response1 =await  _userController.CheckOutProduct(singleProductCheckOutDTO1);           

            ObjectResult result = Assert.IsType<ObjectResult>(response);
            ObjectResult result1 = Assert.IsType<ObjectResult>(response1);

            Assert.Equal(201, result.StatusCode);
            Assert.Equal(404, result1.StatusCode);
   
   
        }
        [Fact]
        public void CheckOutCart_Test()
        {
            CheckOutCart checkOutCart = new CheckOutCart()
            {
                AddressId = Guid.Parse("E74C1BAA-1AE1-4425-9925-DF2054ED083E"),
                PaymentId = Guid.Parse("377D66FB-EACF-4637-9CC2-069AEF71FB4B")
            };

            CheckOutCart checkOutCart1 = new CheckOutCart()
            {
                AddressId = Guid.Parse("DF16FC14-296F-4E48-910D-AFA7BA1E747F"),
                PaymentId = Guid.Parse("377D66FB-EACF-4637-9CC2-069AEF71FB4B")
            };
            ActionResult response = _userController.CheckOutCart(checkOutCart).Result;
            ActionResult response1 = _userController.CheckOutCart(checkOutCart1).Result;

            ObjectResult result = Assert.IsType<ObjectResult>(response);
            ObjectResult result1 = Assert.IsType<ObjectResult>(response1);

            Assert.Equal(201, result.StatusCode);
            Assert.Equal(404, result1.StatusCode);     
        }
    }
}
