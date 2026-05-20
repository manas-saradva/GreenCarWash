using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ReviewServiceTests
    {
        private Mock<IReviewRepository> _reviewRepo;
        private Mock<IOrderRepository> _orderRepo;
        private Mock<IUserRepository> _userRepo;
        private Mock<IWasherRepository> _washerRepo;
        private ReviewService _service;

        [SetUp]
        public void Setup()
        {
            _reviewRepo = new Mock<IReviewRepository>();
            _orderRepo = new Mock<IOrderRepository>();
            _userRepo = new Mock<IUserRepository>();
            _washerRepo = new Mock<IWasherRepository>();

            _service = new ReviewService(_reviewRepo.Object, _orderRepo.Object, _userRepo.Object, _washerRepo.Object);
        }

        [Test]
        public async Task AddReviewAsync_ValidCompletedOrder_AddsReview()
        {
            int custId = 1;
            int washerId = 2;
            int orderId = 10;
            var req = new ReviewRequestDto { OrderId = orderId, WasherId = washerId, Rating = 5, Comment = "Great" };

            var order = new Order { OrderId = orderId, CustomerId = custId, WasherId = washerId, Status = OrderStatus.Completed };
            _orderRepo.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync(order);

            _reviewRepo.Setup(x => x.GetByWasherIdAsync(washerId)).ReturnsAsync(new List<Review> { new Review { Rating = 5 } });
            _washerRepo.Setup(x => x.GetByIdAsync(washerId)).ReturnsAsync(new Washer { WasherId = washerId });
            _userRepo.Setup(x => x.GetByIdAsync(custId)).ReturnsAsync(new Customer { Name = "John" });

            await _service.AddReviewAsync(custId, req);

            _reviewRepo.Verify(x => x.AddAsync(It.IsAny<Review>()), Times.Once);
            _washerRepo.Verify(x => x.UpdateAsync(It.IsAny<Washer>()), Times.Once);
        }

        [Test]
        public void AddReviewAsync_OrderNotFound_ShouldThrowException()
        {
            int custId = 1;
            var req = new ReviewRequestDto { OrderId = 99, WasherId = 2, Rating = 5, Comment = "Great" };
            
            _orderRepo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((Order?)null);

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.AddReviewAsync(custId, req));
        }

        [Test]
        public void AddReviewAsync_OrderNotCompleted_ShouldThrowException()
        {
            int custId = 1;
            var req = new ReviewRequestDto { OrderId = 10, WasherId = 2, Rating = 5, Comment = "Great" };
            var order = new Order { OrderId = 10, CustomerId = custId, WasherId = 2, Status = OrderStatus.Pending };
            
            _orderRepo.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(order);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.AddReviewAsync(custId, req));
        }

        [Test]
        public void AddReviewAsync_WasherMismatch_ShouldThrowException()
        {
            int custId = 1;
            var req = new ReviewRequestDto { OrderId = 10, WasherId = 99, Rating = 5, Comment = "Great" };
            var order = new Order { OrderId = 10, CustomerId = custId, WasherId = 2, Status = OrderStatus.Completed }; 
            
            _orderRepo.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(order);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.AddReviewAsync(custId, req));
        }
    }
}
