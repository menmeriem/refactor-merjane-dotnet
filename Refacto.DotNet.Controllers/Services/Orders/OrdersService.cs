using AutoMapper;
using Refacto.DotNet.Controllers.Dtos.Orders;
using Refacto.DotNet.Controllers.Dtos.Product;
using Refacto.DotNet.Controllers.Repositories.Orders;

namespace Refacto.DotNet.Controllers.Services.Orders
{
    public class OrdersService : IOrdersService
    {

        private readonly IOrdersRepository _ordersRepository;
        private readonly IProductService _productService;

        private readonly IMapper _mapper;


        public OrdersService(IOrdersRepository ordersRepository,IMapper mapper,IProductService productService) 
        { 
            _ordersRepository = ordersRepository;
            _mapper = mapper;
            _productService = productService;
        
        }
        public OrderDto? GetAllOrdersById(long orderId)
        {

            var order= _ordersRepository.GetAllOrdersById(orderId);
            return _mapper.Map<OrderDto>(order);
                
        }

        public ProcessOrderResponse ProcessOrder(long orderId)
        {
            var order = GetAllOrdersById(orderId);

            if (order == null)  throw new ArgumentNullException();



            foreach (var product in order.Items!)
            {
                 ProductDto productDto = _mapper.Map<ProductDto>(product);
                _productService.ProcessProduct(productDto);
            }
            return new ProcessOrderResponse(order.Id);
        }
    }
}
