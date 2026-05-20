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
    public class AdminServiceTests
    {
        private Mock<IAdminRepository> _adminRepo;
        private Mock<IWasherRepository> _washerRepo;
        private Mock<IOrderRepository> _orderRepo;
        private Mock<IPromoCodeRepository> _promoRepo;
        private Mock<IServicePlanRepository> _planRepo;
        private Mock<IAddOnRepository> _addOnRepo;
        private AdminService _service;

        [SetUp]
        public void Setup()
        {
            _adminRepo = new Mock<IAdminRepository>();
            _washerRepo = new Mock<IWasherRepository>();
            _orderRepo = new Mock<IOrderRepository>();
            _promoRepo = new Mock<IPromoCodeRepository>();
            _planRepo = new Mock<IServicePlanRepository>();
            _addOnRepo = new Mock<IAddOnRepository>();

            _service = new AdminService(
                _adminRepo.Object,
                _washerRepo.Object,
                _orderRepo.Object,
                _promoRepo.Object,
                _planRepo.Object,
                _addOnRepo.Object);
        }

        [Test]
        public async Task GetCustomersAsync_ReturnsAllCustomers()
        {
            var customers = new List<Customer>
            {
                new Customer { CustomerId = 1, Name = "Alice" },
                new Customer { CustomerId = 2, Name = "Bob" }
            };
            _adminRepo.Setup(x => x.GetAllCustomersAsync()).ReturnsAsync(customers);

            var result = await _service.GetCustomersAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task AssignWasherAsync_ValidOrderAndWasher_AssignsSuccessfully()
        {
            int orderId = 1;
            int washerId = 5;
            var order = new Order { OrderId = orderId, Status = OrderStatus.Pending };
            var washer = new Washer { WasherId = washerId, Name = "John" };

            _orderRepo.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync(order);
            _washerRepo.Setup(x => x.GetByIdAsync(washerId)).ReturnsAsync(washer);

            await _service.AssignWasherAsync(orderId, washerId);

            Assert.AreEqual(washerId, order.WasherId);
            Assert.AreEqual(OrderStatus.Accepted, order.Status);
            _orderRepo.Verify(x => x.UpdateAsync(order), Times.Once);
        }

        [Test]
        public void AssignWasherAsync_OrderNotFound_ThrowsKeyNotFoundException()
        {
            _orderRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Order?)null);

            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _service.AssignWasherAsync(999, 1));
        }

        [Test]
        public void AssignWasherAsync_WasherNotFound_ThrowsKeyNotFoundException()
        {
            var order = new Order { OrderId = 1, Status = OrderStatus.Pending };
            _orderRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(order);
            _washerRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Washer?)null);

            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _service.AssignWasherAsync(1, 999));
        }

        [Test]
        public async Task CreatePromoCodeAsync_ValidRequest_CreatesPromoCode()
        {
            var req = new PromoCodeRequestDto
            {
                Code = "SAVE20",
                DiscountPercent = 20,
                ExpiryDate = DateTime.UtcNow.AddDays(30),
                MaxUses = 100
            };

            await _service.CreatePromoCodeAsync(req);

            _promoRepo.Verify(x => x.AddAsync(It.IsAny<Promo_code>()), Times.Once);
        }

        [Test]
        public void AssignWasherAsync_OrderNotPending_ShouldThrowInvalidOperationException()
        {
            var order = new Order { OrderId = 1, Status = OrderStatus.InProgress };
            var washer = new Washer { WasherId = 1, Name = "John" };
            
            _orderRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(order);
            _washerRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(washer);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.AssignWasherAsync(1, 1));
        }

        [Test]
        public async Task GetCustomersAsync_WhenNoCustomers_ReturnsEmptyList()
        {
            _adminRepo.Setup(x => x.GetAllCustomersAsync()).ReturnsAsync(new List<Customer>());

            var result = await _service.GetCustomersAsync();

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }
    }
}
