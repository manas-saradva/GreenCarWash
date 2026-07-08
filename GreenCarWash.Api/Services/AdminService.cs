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
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepo;
        private readonly IWasherRepository _washerRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IPromoCodeRepository _promoRepo;
        private readonly IServicePlanRepository _planRepo;
        private readonly IAddOnRepository _addOnRepo;

        public AdminService(IAdminRepository adminRepo,IWasherRepository washerRepo, IOrderRepository orderRepo,
                            IPromoCodeRepository promoRepo, IServicePlanRepository planRepo, IAddOnRepository addOnRepo)
        {
            _adminRepo = adminRepo;
            _washerRepo = washerRepo;
            _orderRepo = orderRepo;
            _promoRepo = promoRepo;
            _planRepo = planRepo;
            _addOnRepo = addOnRepo;
        }

        public Task<List<Customer>> GetCustomersAsync()
        {
            return _adminRepo.GetAllCustomersAsync();
        }

        public Task<List<Washer>> GetWashersAsync()
        {
            return _adminRepo.GetAllWashersAsync();
        }

        public async Task<List<OrderResponseDto>> GetOrdersAsync()
        {
            var orders = await _adminRepo.GetAllOrdersAsync();

            return orders.Select(o => new OrderResponseDto
            {
                OrderId = o.OrderId,
                Status = o.Status.ToString(),
                CustomerName = o.Customer?.Name ?? "",
                WasherName = o.Washer?.Name ?? "",
                CarDetails = o.Car == null ? "" : 
                    $"{o.Car.Make} {o.Car.Model} ({o.Car.Year}) - {o.Car.LicensePlate}",
                PlanName = o.ServicePlan?.Name ?? "",
                TotalAmount = o.TotalAmount,
                ScheduledAt = o.ScheduledAt,
                Location = o.Location ?? ""
            }).ToList();
        }

        public async Task AssignWasherAsync(int orderId,int washerId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if(order == null)
            {
                throw new KeyNotFoundException("Order not found");
            }
            if(order.Status != Enums.OrderStatus.Pending)
            {
                throw new InvalidOperationException("Only pending orders can be assigned");
            }
            
            var washer = await _washerRepo.GetByIdAsync(washerId);
            if(washer == null){
                throw new KeyNotFoundException("Washer not found");
            }

            order.WasherId = washerId;
            order.Status = Enums.OrderStatus.Accepted;
            await _orderRepo.UpdateAsync(order); 
        }

        public async Task<ReportResponseDto> GetReportsAsync()
        {
            var orders = await _adminRepo.GetAllOrdersAsync();
            var washers = await _adminRepo.GetAllWashersAsync();

            var report = new ReportResponseDto
            {
                TotalOrders = orders.Count,
                TotalRevenue = orders.Where(o => o.Status == Enums.OrderStatus.Completed).Sum(o => o.TotalAmount),
                CompletedOrders = orders.Count(o => o.Status == Enums.OrderStatus.Completed),
                CancelledOrders = orders.Count(o => o.Status == Enums.OrderStatus.Cancelled),
                PendingOrders = orders.Count(o => o.Status == Enums.OrderStatus.Pending),
                TopWashers = washers.OrderByDescending(w => w.AverageRating).Take(5).Select(w => new WasherPerformanceDto
                {
                   WasherId = w.WasherId,
                   Name = w.Name,
                   AverageRating = w.AverageRating,
                   CompletedOrders = orders.Count(o => o.WasherId == w.WasherId && o.Status == Enums.OrderStatus.Completed) 
                }).ToList()
            };

            return report;
        }

        public async Task CreatePromoCodeAsync(PromoCodeRequestDto request)
        {
            var promo = new Promo_code
            {
                Code = request.Code,
                DiscountPercent = request.DiscountPercent,
                ExpiryDate = request.ExpiryDate,
                MaxUses = request.MaxUses
            };
            await _promoRepo.AddAsync(promo);
        }

        public async Task UpdatePromoCodeAsync(int id,PromoCodeRequestDto request)
        {
            var promo = await _promoRepo.GetByCodeAsync(request.Code);
            if(promo != null)
            {
                promo.DiscountPercent = request.DiscountPercent;
                promo.ExpiryDate = request.ExpiryDate;
                promo.MaxUses = request.MaxUses;
                promo.IsActive = request.IsActive;
                await _promoRepo.UpdateAsync(promo);
            }else{
                throw new KeyNotFoundException("Promo code not found");
            }
        }

        public async Task<ServicePlan> CreateServicePlanAsync(CreatePlanRequestDto request)
        {
            var plan = new ServicePlan
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                DurationMinutes = request.DurationMinutes,
                IsActive = request.IsActive
            };
            return await _planRepo.AddAsync(plan);
        }

        public async Task<ServicePlan> UpdateServicePlanAsync(int id, CreatePlanRequestDto request)
        {
            var plan = await _planRepo.GetByIdAsync(id);
            if(plan == null)
            {
                throw new KeyNotFoundException("Service plan not found");
            }

            plan.Name = request.Name;
            plan.Description = request.Description;
            plan.Price = request.Price;
            plan.DurationMinutes = request.DurationMinutes;
            plan.IsActive = request.IsActive;
            return await _planRepo.UpdateAsync(plan);
        }

        public async Task<Add_on> CreateAddOnAsync(CreateAddOnRequestDto request)
        {
            var addOn = new Add_on
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                IsActive = request.IsActive
            };
            return await _addOnRepo.AddAsync(addOn);
        }

        public async Task<Add_on> UpdateAddOnAsync(int id, CreateAddOnRequestDto request)
        {
            var addOn = await _addOnRepo.GetByIdAsync(id);
            if(addOn == null)
            {
                throw new KeyNotFoundException("Add-on not found");
            }

            addOn.Name = request.Name;
            addOn.Description = request.Description;
            addOn.Price = request.Price;
            addOn.IsActive = request.IsActive;
            await _addOnRepo.UpdateAsync(addOn);
            return addOn;
        }
    }
}