using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Refacto.DotNet.Controllers.Database.Context;
using Refacto.DotNet.Controllers.Dtos.Product;
using Refacto.DotNet.Controllers.Entities;
using Refacto.DotNet.Controllers.Mapper;
using Refacto.DotNet.Controllers.Services;
using Refacto.DotNet.Controllers.Services.Products;

namespace Refacto.DotNet.Controllers.Tests.Services
{
    public class MyUnitTests
    {
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly Mock<AppDbContext> _mockDbContext;
        private readonly IMapper _mapper;
        private readonly ProductService _productService;

        public MyUnitTests()
        {
            _mockNotificationService = new Mock<INotificationService>();
            _mockDbContext = new Mock<AppDbContext>();
            _mockDbContext.Setup(x => x.Products)
                          .ReturnsDbSet(Array.Empty<Product>());

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile( new RefactoringMapper());
            });
            _mapper = config.CreateMapper();

            _productService = new ProductService(
                _mockNotificationService.Object,
                _mockDbContext.Object,
                _mapper
            );
        }

        [Fact]
        public void NotifyDelay_ShouldSaveAndSendNotification()
        {
            // GIVEN
            Product product = new()
            {
                LeadTime = 15,
                Available = 0,
                Type = "NORMAL",
                Name = "RJ45 Cable"
            };

            // WHEN
            _productService.NotifyDelay(product.LeadTime, product);

            // THEN
            Assert.Equal(0, product.Available);
            Assert.Equal(15, product.LeadTime);
            _mockDbContext.Verify(ctx => ctx.SaveChanges(), Times.Once());
            _mockNotificationService.Verify(
                service => service.SendDelayNotification(product.LeadTime, product.Name), Times.Once());
        }

        [Fact]
        public void ProcessProduct_NormalProduct_WithStock_ShouldDecrementAvailable()
        {
            // GIVEN
            var dto = new ProductDto { LeadTime = 15, Available = 5, Type = "NORMAL", Name = "USB Cable" };

            // WHEN
            _productService.ProcessProduct(dto);

            // THEN
            _mockDbContext.Verify(ctx => ctx.SaveChanges(), Times.Once());
        }

        [Fact]
        public void ProcessProduct_NormalProduct_OutOfStock_ShouldNotifyDelay()
        {
            // GIVEN
            var dto = new ProductDto { LeadTime = 15, Available = 0, Type = "NORMAL", Name = "USB Dongle" };

            // WHEN
            _productService.ProcessProduct(dto);

            // THEN
            _mockNotificationService.Verify(
                s => s.SendDelayNotification(15, "USB Dongle"), Times.Once());
        }

        [Fact]
        public void ProcessProduct_ExpirableProduct_NotExpired_ShouldDecrementAvailable()
        {
            // GIVEN
            var dto = new ProductDto
            {
                Available = 5,
                Type = "EXPIRABLE",
                Name = "Butter",
                ExpiryDate = DateTime.Now.AddDays(10)
            };

            // WHEN
            _productService.ProcessProduct(dto);

            // THEN
            _mockDbContext.Verify(ctx => ctx.SaveChanges(), Times.Once());
        }

        [Fact]
        public void ProcessProduct_ExpirableProduct_Expired_ShouldNotifyExpiration()
        {
            // GIVEN
            var expiryDate = DateTime.Now.AddDays(-1);
            var dto = new ProductDto
            {
                Available = 5,
                Type = "EXPIRABLE",
                Name = "Milk",
                ExpiryDate = expiryDate
            };

            // WHEN
            _productService.ProcessProduct(dto);

            // THEN
            _mockNotificationService.Verify(
                s => s.SendExpirationNotification("Milk", expiryDate), Times.Once());
        }

        [Fact]
        public void ProcessProduct_SeasonalProduct_InSeason_ShouldDecrementAvailable()
        {
            // GIVEN
            var dto = new ProductDto
            {
                Available = 10,
                Type = "SEASONAL",
                Name = "Watermelon",
                SeasonStartDate = DateTime.Now.AddDays(-5),
                SeasonEndDate = DateTime.Now.AddDays(60),
                LeadTime = 5
            };

            // WHEN
            _productService.ProcessProduct(dto);

            // THEN
            _mockDbContext.Verify(ctx => ctx.SaveChanges(), Times.Once());
        }

        [Fact]
        public void ProcessProduct_SeasonalProduct_OutOfSeason_ShouldNotifyOutOfStock()
        {
            // GIVEN
            var dto = new ProductDto
            {
                Available = 10,
                Type = "SEASONAL",
                Name = "Grapes",
                SeasonStartDate = DateTime.Now.AddDays(180),
                SeasonEndDate = DateTime.Now.AddDays(240),
                LeadTime = 5
            };

            // WHEN
            _productService.ProcessProduct(dto);

            // THEN
            _mockNotificationService.Verify(
                s => s.SendOutOfStockNotification("Grapes"), Times.Once());
        }

        [Fact]
        public void ProcessProduct_SeasonalProduct_LeadTimeExceedsSeason_ShouldNotifyOutOfStock()
        {
            // GIVEN
            var dto = new ProductDto
            {
                Available = 10,
                Type = "SEASONAL",
                Name = "Watermelon",
                SeasonStartDate = DateTime.Now.AddDays(-10),
                SeasonEndDate = DateTime.Now.AddDays(5),
                LeadTime = 30
            };

            // WHEN
            _productService.ProcessProduct(dto);

            // THEN
            _mockNotificationService.Verify(
                s => s.SendOutOfStockNotification("Watermelon"), Times.Once());
        }
    }
}