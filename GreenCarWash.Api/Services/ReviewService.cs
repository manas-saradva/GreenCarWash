using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenCarWash.Api.DTOs.RequestDtos;
using GreenCarWash.Api.DTOs.ResponseDtos;
using GreenCarWash.Api.Interfaces;
using GreenCarWash.Api.Models;

namespace GreenCarWash.Api.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IWasherRepository _washerRepo;

        public ReviewService(IReviewRepository reviewRepo, IOrderRepository orderRepo, IUserRepository userRepo, IWasherRepository washerRepo)
        {
            _reviewRepo = reviewRepo;
            _orderRepo = orderRepo;
            _washerRepo = washerRepo;
        }

        public async Task AddReviewAsync(int customerId,ReviewRequestDto request)
        {
            var order = await _orderRepo.GetByIdAsync(request.OrderId);
            if (order == null || order.CustomerId != customerId)
            {
                throw new KeyNotFoundException("Order not found or not owned by customer.");
            }
            if (order.Status != Enums.OrderStatus.Completed)
            {
                throw new System.InvalidOperationException("Cannot review an incomplete order.");
            }

            if (order.WasherId != request.WasherId)
            {
                throw new InvalidOperationException("Washer does not match the order.");
            }

            var existingReview = await _reviewRepo.GetByOrderIdAsync(request.OrderId);
            if(existingReview != null)
            {
                throw new InvalidOperationException("This order has already been reviewed.");
            }
        
            var review = new Review
            {
                OrderId = request.OrderId,
                CustomerId = customerId,
                WasherId = request.WasherId,
                Rating = request.Rating,
                Comment = request.Comment
            };
            await _reviewRepo.AddAsync(review);

            
            var washer = await _washerRepo.GetByIdAsync(order.WasherId.Value);
            if (washer != null)
            {
                var avgRating = await _reviewRepo.GetAverageRatingAsync(order.WasherId.Value);
                washer.AverageRating = (decimal)avgRating;
                await _washerRepo.UpdateAsync(washer);
            }
        }

        public async Task<List<ReviewResponseDto>> GetWasherReviewsAsync(int washerId)
        {
            var reviews = await _reviewRepo.GetByWasherIdAsync(washerId);
            return reviews.Select(r => new ReviewResponseDto
            {
                ReviewId = r.ReviewId,
                Rating = r.Rating,
                Comment = r.Comment,
                CustomerName = r.Customer?.Name ?? "",
                CreatedAt = r.CreatedAt
            }).ToList();
        }
    }
}