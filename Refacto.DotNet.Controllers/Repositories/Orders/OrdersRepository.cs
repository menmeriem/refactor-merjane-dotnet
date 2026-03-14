using Microsoft.EntityFrameworkCore;
using Refacto.DotNet.Controllers.Database.Context;
using Refacto.DotNet.Controllers.Entities;

namespace Refacto.DotNet.Controllers.Repositories.Orders
{
    public class OrdersRepository : IOrdersRepository
    {

        private readonly AppDbContext _dbContext;

        public OrdersRepository( AppDbContext dbContext)
        {
            _dbContext = dbContext;

        }

        public Order? GetAllOrdersById(long orderId)
        {
            return _dbContext.Orders
                .Include(o => o.Items)
                .SingleOrDefault(o => o.Id == orderId);

        }
    }
}
