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
                    options.IncludeScopes = true; //AddAuthentication
                });
            });
            IHost host = hostBuilder.Build();
            ILogger<OrderController> _logger = host.Services.GetRequiredService<ILogger<OrderController>>();

            MapperConfiguration mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new Mapper());
            });


            // need to have access to the context
            Claim claim = new Claim("role", "User");
            Claim claim1 = new Claim("Id", "8d0c1df7-a887-4453-8af3-799e4a7ed013");
            ClaimsIdentity identity = new ClaimsIdentity(new[] { claim, claim1 }, "BasicAuthentication"); // this uses basic auth
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            GenericIdentity identityy = new GenericIdentity("some name", "test");
            ClaimsPrincipal contextUser = new ClaimsPrincipal(identity); //add claims as needed

            //...then set user and other required properties on the httpContext as needed
            DefaultHttpContext httpContext = new DefaultHttpContext()
            {
                User = contextUser
            };

            //Controller needs a controller context to access HttpContext
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
            string path = @"C:\Users\Hp\source\repos\OrderService\OrderService\Entities\UnitTest_Files\TextFile.csv";
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
                    Bill = new List<Bill>()
                };
                if(row[2] == "1")
                {
                    Bill bill = new Bill()
                    {
                        Id = int.Parse(row[2]),
                        OrderValue = int.Parse(row[3]),
                        PaymentId = Guid.Parse(row[4]),
                        AddressId = Guid.Parse(row[5]),
                        CartId = Guid.Parse(row[0])
                    };
                    cart.Bill.Add(bill);
                }
                WishList wishList = new WishList()
                {
                    Id = Guid.Parse(row[9]),
                    Name = row[10],
                    UserId = Guid.Parse(row[1]),
                    WishListProduct = new List<WishListProduct>()
                };
                WishListProduct wishListProduct = new WishListProduct()
                {
                    Id = Guid.Parse(row[11]),
                    ProductId = Guid.Parse(row[8]),
                    WishListId=Guid.Parse(row[9]) 
                };
                wishList.WishListProduct.Add(wishListProduct);
                Product product = new Product()
                {
                    Id = Guid.Parse(row[6]),
                    ProductId = Guid.Parse(row[8]),
                    Quantity = int.Parse(row[7]),
                    CartId= Guid.Parse(row[0])
                };
                
                cart.Product.Add(product);
                _context.WishList.Add(wishList);
                _context.Add(cart);
            }
            _context.SaveChanges();
        }

        [Fact]
        public void AddProductCart_Test()
        {
            ProductToCartDTO productToCartDTO = new ProductToCartDTO()
            {
                Quantity = 1,
                ProductId = Guid.Parse("B08AD56A-46E8-40B4-88A9-1A2CE2DFF94A"),
                CategoryId = Guid.Parse("A0BB058B-5217-4217-9FB2-4F75E15D2CCF")
            };
            ActionResult response = _userController.AddProductCart(productToCartDTO).Result;
            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, result.StatusCode);
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
                ProductId = Guid.Parse("B08AD56A-46E8-40B4-88A9-1A2CE2DFF94A"),
                Quantity = 1
            };
            IActionResult response = _userController.UpdateQuantity(updateCart);
            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, result.StatusCode);
        }
        [Fact]
        public void CreateWishListProduct_Test()
        {
            NewWishListProduct newWishListProduct = new NewWishListProduct()
            {
                ProductId = Guid.Parse("C4EC41BB-A772-4909-8AEE-C3010F423132"),
                Name = "Wish_1",
                CategoryId = Guid.Parse("5B3D6E85-4B03-4856-BBB8-79C3D5C3AB1F")
            };
            ActionResult response = _userController.CreateWishListProduct(newWishListProduct).Result;
            ObjectResult result = Assert.IsType<ObjectResult>(response);
            Assert.Equal(201, result.StatusCode);
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
            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, result.StatusCode);
        }
        [Fact]
        public void AddProductWishList_Test()
        {
            WishListproduct wishListproduct = new WishListproduct()
            {
                WishListId = Guid.Parse("747f9ed2-47ff-4467-b635-b6079c935f88"),
                ProductId = Guid.Parse("C4EC41BB-A772-4909-8AEE-C3010F423132"),
                CategoryId = Guid.Parse("5B3D6E85-4B03-4856-BBB8-79C3D5C3AB1F")
            };
            ActionResult response = _userController.AddProductWishList(wishListproduct).Result;
            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, result.StatusCode);
        }
        [Fact]
        public void DeleteProductInWishList_Test()
        {
            IActionResult response = _userController.DeleteProductInWishList(Guid.Parse("747f9ed2-47ff-4467-b635-b6079c935f88"),
                Guid.Parse("b08ad56a-46e8-40b4-88a9-1a2ce2dff94a"));
            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, result.StatusCode);
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
            ActionResult<List<WishListProductDTO>> response  = _userController.GetWishListProducts(id).Result;
            OkObjectResult result = Assert.IsType<OkObjectResult>(response.Result);
            Assert.Equal(200, result.StatusCode);
        }
        [Fact]
        public void OrderDetails_Test()
        {
            CheckOutCart cart = new CheckOutCart()
            {
                AddressId =Guid.Parse("9e610e23-4ad4-4d05-9dbb-5de774f72f91"),
                PaymentId=Guid.Parse("a334b297-3cc6-4d30-a304-0d95f7299064")
            };
            ActionResult response = _userController.GetProductsInCart().Result;
            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, result.StatusCode);
        }
        [Fact]
        public void OrderDetail_Test()
        {
            IActionResult response = _userController.OrderDetail(1);
            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, result.StatusCode);
        }
        [Fact]
        public void DeleteProductCart_Test()
        {
            CheckOutCart cart = new CheckOutCart()
            {
                AddressId = Guid.Parse("9e610e23-4ad4-4d05-9dbb-5de774f72f91"),
                PaymentId = Guid.Parse("a334b297-3cc6-4d30-a304-0d95f7299064")
            };
            IActionResult response = _userController.DeleteProductCart(Guid.Parse("b08ad56a-46e8-40b4-88a9-1a2ce2dff94a"));
            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, result.StatusCode);
        }
        [Fact]
        public void MoveProductToCart_Test()
        {
            WishListToCart cart = new WishListToCart()
            {
                ProductId = "b08ad56a-46e8-40b4-88a9-1a2ce2dff94a",
                WishListId ="747f9ed2-47ff-4467-b635-b6079c935f88"
            };
            IActionResult response = _userController.MoveProductToCart(cart);
            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, result.StatusCode);
        }
        [Fact]
        public async Task OutProduct_Test()
        {
            SingleProductCheckOutDTO singleProductCheckOutDTO = new SingleProductCheckOutDTO()
            {
                AddressId = Guid.Parse("AFFB4EA1-0A8F-401C-848C-7436B98A1278"),
                PaymentId = Guid.Parse("955B72B8-8DE2-4F4B-9612-588131EA9C3A"),
                ProductId =Guid.Parse("B08AD56A-46E8-40B4-88A9-1A2CE2DFF94A"),
                Quantity =1,
                CategoryId = Guid.Parse("A0BB058B-5217-4217-9FB2-4F75E15D2CCF")
            };
            ActionResult response =await  _userController.CheckOutProduct(singleProductCheckOutDTO);
            ObjectResult result = Assert.IsType<ObjectResult>(response);
            Assert.Equal(201, result.StatusCode);
        }
        [Fact]
        public void CheckOutCart_Test()
        {
            CheckOutCart checkOutCart = new CheckOutCart()
            {
                AddressId = Guid.Parse("AFFB4EA1-0A8F-401C-848C-7436B98A1278"),
                PaymentId = Guid.Parse("955B72B8-8DE2-4F4B-9612-588131EA9C3A")
            };
            ActionResult response = _userController.CheckOutCart(checkOutCart).Result;
            ObjectResult result = Assert.IsType<ObjectResult>(response);
            Assert.Equal(201, result.StatusCode);     
        }
    }
}
