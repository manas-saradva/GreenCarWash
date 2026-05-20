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
    public class WasherServiceTests
    {
        private Mock<IOrderRepository> _orderRepo;
        private Mock<IWasherRepository> _washerRepo;
        private WasherService _service;

        [SetUp]
        public void Setup()
        {
            _orderRepo = new Mock<IOrderRepository>();
            _washerRepo = new Mock<IWasherRepository>();
            _service = new WasherService(_orderRepo.Object, _washerRepo.Object);
        }

        [Test]
        public async Task AcceptOrderAsync_ValidOrder_UpdatesStatus()
        {
            int washerId = 5;
            int orderId = 100;
            var order = new Order { OrderId = orderId, Status = OrderStatus.Pending };

            _orderRepo.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync(order);

            await _service.AcceptOrderAsync(orderId, washerId);

            Assert.AreEqual(OrderStatus.Accepted, order.Status);
            Assert.AreEqual(washerId, order.WasherId);
            _orderRepo.Verify(x => x.UpdateAsync(order), Times.Once);
        }

        [Test]
        public void AcceptOrderAsync_InvalidStatus_ThrowsException()
        {
            int washerId = 5;
            int orderId = 100;
            var order = new Order { OrderId = orderId, Status = OrderStatus.InProgress }; // Not Pending

            _orderRepo.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync(order);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.AcceptOrderAsync(orderId, washerId));
        }

        [Test]
        public void AcceptOrderAsync_OrderNotFound_ShouldThrowException()
        {
            int washerId = 5;
            int orderId = 99;
            
            _orderRepo.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync((Order?)null);

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.AcceptOrderAsync(orderId, washerId));
        }
    }
}
