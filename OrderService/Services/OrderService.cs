using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Order_Service.Contracts;
using Order_Service.Entities.Dtos;
using Order_Service.Entity.DTo;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Order_Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _context;

        public OrderService(IHttpClientFactory httpClientFactory, IHttpContextAccessor context)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
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


        public async   Task<ErrorDTO> IsTheproductExists(Guid productId,Guid categoryId)
        {

            using var client = _httpClientFactory.CreateClient("product");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
          //  HttpContent content = new StringContent(productId.ToString());
            var userId = _context.HttpContext.User.Claims.First(i => i.Type == "Id").Value;

            string accessToken = AccessToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",accessToken);
            var response = await client.GetAsync($"/api/product/ocelot?id={productId}&categoryId={categoryId}");

            var result =await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject(result);
            return new ErrorDTO();
        }
    }
}
