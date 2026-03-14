using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Refacto.DotNet.Controllers.Database.Context;
using Refacto.DotNet.Controllers.Dtos.Product;
using Refacto.DotNet.Controllers.Entities;
using Refacto.DotNet.Controllers.Enums;

using Refacto.DotNet.Controllers.Services.Impl;

namespace Refacto.DotNet.Controllers.Services.Products
{
    public class ProductService: IProductService
    {
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;


        public ProductService(INotificationService notificationService, AppDbContext ctx, IMapper mapper)
        {
            _notificationService = notificationService;
            _dbContext = ctx;
            _mapper = mapper;
        }
            
        public void ProcessProduct(ProductDto productDto)
           {
              Product product = _mapper.Map<Product>(productDto);

             if (!Enum.TryParse(product.Type, out ProductType productType))
                throw new ArgumentException($"Unknown product type: {product.Type}");

                   switch (productType)
                   {
                        case ProductType.NORMAL:
                            HandleNormalProduct(product);
                            break;
                        case ProductType.SEASONAL:
                            HandleSeasonalProduct(product);
                            break;
                        case ProductType.EXPIRABLE:
                            HandleExpiredProduct(product);
                            break;
                   }
        }

        private void HandleNormalProduct(Product product)
        {
            if (product.Available > 0)
            {
                product.Available -= 1;
                _dbContext.SaveChanges();
            }
            else if (product.LeadTime > 0)
            {
                NotifyDelay(product.LeadTime, product);
            }
        }


        public void NotifyDelay(int leadTime, Product p)
        {
            p.LeadTime = leadTime;
            _ = _dbContext.SaveChanges();
            _notificationService.SendDelayNotification(leadTime, p.Name);
        }

        private void HandleSeasonalProduct(Product product)
        {
            bool isInSeason = DateTime.Now.Date > product.SeasonStartDate
                           && DateTime.Now.Date < product.SeasonEndDate;

            bool leadTimeExceedsSeason = DateTime.Now.AddDays(product.LeadTime) > product.SeasonEndDate;

            if (isInSeason && product.Available > 0 && !leadTimeExceedsSeason)
            {
                product.Available -= 1;
                _dbContext.SaveChanges();
            }
            else if (leadTimeExceedsSeason || product.SeasonStartDate > DateTime.Now)
            {
                _notificationService.SendOutOfStockNotification(product.Name);
                product.Available = 0;
                _dbContext.SaveChanges();
            }
            else
            {
                NotifyDelay(product.LeadTime, product);
            }
        }
        private void HandleExpiredProduct(Product product)
        {
            bool isExpired = product.ExpiryDate < DateTime.Now.Date;

            if (product.Available > 0 && !isExpired)
            {
                product.Available -= 1;
                _dbContext.SaveChanges();
            }
            else
            {
                _notificationService.SendExpirationNotification(product.Name, (DateTime)product.ExpiryDate!);
                product.Available = 0;
                _dbContext.SaveChanges();
            }
        }

    }
}
