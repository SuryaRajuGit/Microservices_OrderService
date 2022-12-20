using Microsoft.AspNetCore.Mvc.ModelBinding;
using Order_Service.Entity.DTo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Contracts
{
    public interface IOrderService
    {
        public ErrorDTO ModelStateInvalid(ModelStateDictionary ModelState);

        public Task<ErrorDTO> IsTheproductExists(Guid productId,Guid categoryId);
    }
}
