using Refacto.DotNet.Controllers.Dtos.Product;

namespace Refacto.DotNet.Controllers.Dtos.Orders
{
    public class OrderDto
    {
        public long Id { get; set; }
        public ICollection<ProductDto>? Items { get; set; }
    }
}
