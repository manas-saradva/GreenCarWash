using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenCarWash.Api.DTOs.RequestDtos;
using GreenCarWash.Api.DTOs.ResponseDtos;
using GreenCarWash.Api.Enums;
using GreenCarWash.Api.Interfaces;
using GreenCarWash.Api.Models;

namespace GreenCarWash.Api.Services
{
    public class WasherService : IWasherService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IWasherRepository _washerRepo;

        public WasherService(IOrderRepository orderRepo, IWasherRepository washerRepo)
        {
            _orderRepo = orderRepo;
            _washerRepo = washerRepo;
        }

        private async Task UpdateOrderStatusAsync(int washerId, int orderId, OrderStatus requiredState, OrderStatus newState)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("Order not found");
            }
            if(order.WasherId != null && order.WasherId != washerId)
            {
                throw new InvalidOperationException("Order assigned to another washer");
            }
            if (order.Status != requiredState)
            {
                throw new InvalidOperationException($"Order must be in {requiredState} state to perform this action");
            }
            if (newState == OrderStatus.Accepted)
            {
                order.WasherId = washerId;
            } 
            order.Status = newState;
            await _orderRepo.UpdateAsync(order);
        }

        public async Task AcceptOrderAsync(int orderId, int washerId) => 
            await UpdateOrderStatusAsync(washerId, orderId, OrderStatus.Pending, OrderStatus.Accepted);

        public async Task DeclineOrderAsync(int orderId, int washerId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }
            if (order.WasherId != washerId || order.Status != OrderStatus.Accepted)
            {
                throw new InvalidOperationException("Order cannot be declined.");
            }
            order.WasherId = null;
            order.Status = OrderStatus.Pending;

            await _orderRepo.UpdateAsync(order);
        }

        public async Task StartOrderAsync(int orderId, int washerId) => 
            await UpdateOrderStatusAsync(washerId, orderId, OrderStatus.Accepted, OrderStatus.InProgress);

        public async Task CompleteOrderAsync(int orderId, int washerId)
        {
            await UpdateOrderStatusAsync(washerId, orderId, OrderStatus.InProgress, OrderStatus.Completed);
        }

        public async Task<List<OrderResponseDto>> GetMyOrdersAsync(int washerId)
        {
            var orders = await _orderRepo.GetByWasherAsync(washerId);
            return orders.Select(o => new OrderResponseDto
            {
                OrderId = o.OrderId,
                Status = o.Status.ToString(),
                CustomerName = o.Customer?.Name ?? "",
                WasherName = o.Washer?.Name ?? "",
                CarDetails = o.Car != null ? $"{o.Car.Make} {o.Car.Model} ({o.Car.Year}) - {o.Car.LicensePlate}" : "",
                PlanName = o.ServicePlan?.Name ?? "",
                TotalAmount = o.TotalAmount,
                ScheduledAt = o.ScheduledAt,
                Location = o.Location,
                Notes = o.Notes
            }).ToList();
        }

        public async Task<WasherProfileResponseDto> GetProfileAsync(int washerId)
        {
            var washer = await _washerRepo.GetByIdAsync(washerId);
            if (washer == null)
            {
                throw new KeyNotFoundException("Washer not found");
            } 
            return new WasherProfileResponseDto
            {
                WasherId = washer.WasherId,
                Name = washer.Name,
                Email = washer.Email,
                Phone = washer.Phone,
                IsActive = washer.IsActive,
                AverageRating = washer.AverageRating
            };
        }

        public async Task UpdateProfileAsync(int washerId, UpdateWasherRequestDto request)
        {
            var washer = await _washerRepo.GetByIdAsync(washerId);
            if (washer == null)
            {
                throw new KeyNotFoundException("Washer not found");
            }
            washer.Name = request.Name;
            washer.Phone = request.Phone;
            washer.IsActive = request.IsActive;
            await _washerRepo.UpdateAsync(washer);
        }
    }
}