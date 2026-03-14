using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Refacto.DotNet.Controllers.Database.Context;
using Refacto.DotNet.Controllers.Dtos.Product;
using Refacto.DotNet.Controllers.Services;
using Refacto.DotNet.Controllers.Services.Orders;
using Refacto.DotNet.Controllers.Services.Products;

namespace Refacto.DotNet.Controllers.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
    
        private readonly IOrdersService _orderService;
   

        public OrdersController(IOrdersService orderService)
        {
        
            _orderService = orderService;
        }

        [HttpPost("{orderId}/processOrder")]
        [ProducesResponseType(200)]
        public ActionResult<ProcessOrderResponse> ProcessOrder(long orderId)
        {
            var response = _orderService.ProcessOrder(orderId);
            return Ok(response);
        }


    }
}

