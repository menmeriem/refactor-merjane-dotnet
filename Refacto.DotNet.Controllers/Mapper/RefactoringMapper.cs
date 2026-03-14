using AutoMapper;
using Refacto.DotNet.Controllers.Dtos.Orders;
using Refacto.DotNet.Controllers.Dtos.Product;
using Refacto.DotNet.Controllers.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Refacto.DotNet.Controllers.Mapper
{
    public   class RefactoringMapper : Profile
    {

        public RefactoringMapper() {

            CreateMap<Product, ProductDto>();

            
            CreateMap<ProductDto, Product>();


            CreateMap<OrderDto, Order>();

            CreateMap<Order, OrderDto>();


        }
    }
}
