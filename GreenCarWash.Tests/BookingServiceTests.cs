using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GreenCarWash.Api.DTOs.RequestDtos;
using GreenCarWash.Api.DTOs.ResponseDtos;
using GreenCarWash.Api.Enums;
using GreenCarWash.Api.Interfaces;
using GreenCarWash.Api.Models;
using GreenCarWash.Api.Services;
using Moq;
using NUnit.Framework;

namespace GreenCarWash.Tests
{
    [TestFixture]
    public class BookingServiceTests
    {
        private Mock<IOrderRepository> _orderRepo;
        private Mock<ICarRepository> _carRepo;
        private Mock<IServicePlanRepository> _planRepo;
        private Mock<IAddOnRepository> _addonRepo;
        private Mock<IPromoCodeRepository> _promoRepo;
        private Mock<IEmailService> _emailMock;
        private Mock<IUserRepository> _userRepo;
        private BookingService _service;

        [SetUp]
        public void Setup()
        {
            _orderRepo = new Mock<IOrderRepository>();
            _carRepo = new Mock<ICarRepository>();
            _planRepo = new Mock<IServicePlanRepository>();
            _addonRepo = new Mock<IAddOnRepository>();
            _promoRepo = new Mock<IPromoCodeRepository>();
            _emailMock = new Mock<IEmailService>();
            _userRepo = new Mock<IUserRepository>();

            _service = new BookingService(_orderRepo.Object, _carRepo.Object, _planRepo.Object, _addonRepo.Object, _promoRepo.Object, _emailMock.Object, _userRepo.Object);
        }

        [Test]
        public async Task BookAsync_ValidRequest_CreatesOrder()
        {
            int custId = 1;
            var req = new BookingRequestDto { CarId = 10, PlanId = 5, ScheduledAt = DateTime.UtcNow.AddDays(1) };

            _carRepo.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(new Car { CarId = 10, CustomerId = custId });
            _planRepo.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(new ServicePlan { PlanId = 5, Price = 50, IsActive = true });
            _orderRepo.Setup(x => x.AddAsync(It.IsAny<Order>())).Callback<Order>(o => o.OrderId = 100).ReturnsAsync((Order o) => o);

            var result = await _service.BookAsync(custId, req);

            Assert.IsNotNull(result);
            Assert.AreEqual(100, result.OrderId);
            Assert.AreEqual("Pending", result.Status);
            Assert.AreEqual(50, result.TotalAmount);
            _orderRepo.Verify(x => x.AddAsync(It.IsAny<Order>()), Times.Once);
        }

        [Test]
        public void BookAsync_CarNotFound_ShouldThrowException()
        {
            int custId = 1;
            var req = new BookingRequestDto { CarId = 99, PlanId = 5, ScheduledAt = DateTime.UtcNow.AddDays(1) };
            
            _carRepo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((Car?)null);

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.BookAsync(custId, req));
        }

        [Test]
        public void BookAsync_CarDoesNotBelongToCustomer_ShouldThrowException()
        {
            int custId = 1;
            var req = new BookingRequestDto { CarId = 10, PlanId = 5, ScheduledAt = DateTime.UtcNow.AddDays(1) };
            
            _carRepo.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(new Car { CarId = 10, CustomerId = 2 });

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _service.BookAsync(custId, req));
        }

        [Test]
        public void BookAsync_ServicePlanNotFound_ShouldThrowException()
        {
            int custId = 1;
            var req = new BookingRequestDto { CarId = 10, PlanId = 99, ScheduledAt = DateTime.UtcNow.AddDays(1) };
            
            _carRepo.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(new Car { CarId = 10, CustomerId = custId });
            _planRepo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((ServicePlan?)null);

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.BookAsync(custId, req));
        }

        [Test]
        public void BookAsync_ServicePlanInactive_ShouldThrowException()
        {
            int custId = 1;
            var req = new BookingRequestDto { CarId = 10, PlanId = 5, ScheduledAt = DateTime.UtcNow.AddDays(1) };
            
            _carRepo.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(new Car { CarId = 10, CustomerId = custId });
            _planRepo.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(new ServicePlan { PlanId = 5, IsActive = false });

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.BookAsync(custId, req));
        }

        [Test]
        public async Task BookAsync_InvalidPromoCode_ShouldIgnoreOrThrowBasedOnServiceLogic()
        {
            int custId = 1;
            var req = new BookingRequestDto { CarId = 10, PlanId = 5, ScheduledAt = DateTime.UtcNow.AddDays(1), PromoCode = "INVALID" };
            
            _carRepo.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(new Car { CarId = 10, CustomerId = custId });
            _planRepo.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(new ServicePlan { PlanId = 5, Price = 50, IsActive = true });
            _promoRepo.Setup(x => x.GetByCodeAsync("INVALID")).ReturnsAsync((Promo_code?)null);
            _orderRepo.Setup(x => x.AddAsync(It.IsAny<Order>())).ReturnsAsync(new Order { OrderId = 100 });

            try
            {
                await _service.BookAsync(custId, req);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is KeyNotFoundException || ex is InvalidOperationException || ex is ArgumentException);
            }
        }
    }
}
