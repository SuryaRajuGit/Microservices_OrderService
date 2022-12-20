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
            ErrorDTO isTheProductExists =await  _orderService.IsTheproductExists(productToCartDTO.ProductId,productToCartDTO.CategoryId);
            return Ok();
        }
    }
}
