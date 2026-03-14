using Microsoft.AspNetCore.Mvc;
using Refacto.DotNet.Controllers.Dtos.Orders;
using Refacto.DotNet.Controllers.Dtos.Product;
using Refacto.DotNet.Controllers.Entities;

namespace Refacto.DotNet.Controllers.Services.Orders
{
    public interface IOrdersService
    {
        ProcessOrderResponse ProcessOrder(long orderId);
        OrderDto? GetAllOrdersById(long orderId);
    }
}
