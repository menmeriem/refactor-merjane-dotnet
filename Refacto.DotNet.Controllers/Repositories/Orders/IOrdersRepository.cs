using Refacto.DotNet.Controllers.Entities;

namespace Refacto.DotNet.Controllers.Repositories.Orders
{
    public interface IOrdersRepository
    {
        Order? GetAllOrdersById(long id );
    }
}
