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
    public class BookingService : IBookingService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICarRepository _carRepo;
        private readonly IServicePlanRepository _planRepo;
        private readonly IAddOnRepository _addOnRepo;
        private readonly IPromoCodeRepository _promoRepo;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepo;

        public BookingService(IOrderRepository orderRepo,ICarRepository carRepo,IServicePlanRepository planRepo,IAddOnRepository addOnRepo,
                                IPromoCodeRepository promoRepo,IEmailService emailService,IUserRepository userRepo)
        {
            _orderRepo = orderRepo;
            _carRepo = carRepo;
            _planRepo = planRepo;
            _addOnRepo = addOnRepo;
            _promoRepo = promoRepo;
            _emailService = emailService;
            _userRepo = userRepo;
        }

        public async Task<OrderResponseDto> BookAsync(int customerId,BookingRequestDto request)
        {
            var car = await _carRepo.GetByIdAsync(request.CarId);
            if(car == null )
            {
                throw new KeyNotFoundException("Car not found");
            }

            if(car.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException("Car does not belong to customer");
            }

            var plan = await _planRepo.GetByIdAsync(request.PlanId);
            if(plan == null)
            {
                throw new KeyNotFoundException("Service plan not found");
            }

            if (!plan.IsActive)
            {
                throw new InvalidOperationException("Service plan is inactive");
            }

            decimal totalAmount = plan.Price;
            string? addOnName = null;
            decimal addOnPrice = 0;
            int? addOnId = null;

            if(request.AddOnId.HasValue)
            {
                var addOn = await _addOnRepo.GetByIdAsync(request.AddOnId.Value);
                if(addOn != null && addOn.IsActive)
                {
                    totalAmount += addOn.Price;
                    addOnName = addOn.Name;
                    addOnPrice = addOn.Price;
                    addOnId = addOn.AddOnId;
                }
            }

            int? promoId = null;

            if (!string.IsNullOrWhiteSpace(request.PromoCode))
            {
                var promo = await _promoRepo.GetByCodeAsync(request.PromoCode);

                if(promo != null && promo.IsActive && promo.ExpiryDate > DateTime.UtcNow)
                {
                    totalAmount -= totalAmount * (promo.DiscountPercent/100m);
                    promoId = promo.PromoCodeId;
                }
            }

            var order = new Order
            {
                CustomerId = customerId,
                CarId = request.CarId,
                PlanId = request.PlanId,
                PromoCodeId = promoId,
                AddOnId = addOnId,
                Status = Enums.OrderStatus.Pending,
                ScheduledAt = request.ScheduledAt,
                Location = request.Location,
                Notes = request.Notes,
                TotalAmount = totalAmount
            };

            await _orderRepo.AddAsync(order);


            var customer = await _userRepo.GetByIdAsync(customerId);

            if(customer != null)
            {
                var body = $"Booking Confirmed\nOrder ID: {order.OrderId}\nService Plan: {plan.Name}\nScheduled At: {order.ScheduledAt}\nLocation: {order.Location}\nTotal Amount: ₹{order.TotalAmount}";

                await _emailService.SendEmailAsync(customer.Email,"GreenCarWash Booking Confirmation",body); 
            }

            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                Status = order.Status.ToString(),
                CustomerName = customer?.Name ?? "",
                WasherName = "",
                CarDetails = $"{car.Make} {car.Model} ({car.Year}) - {car.LicensePlate}",
                PlanName = plan.Name,
                AddOnName = addOnName,
                AddOnPrice = addOnPrice,
                TotalAmount = order.TotalAmount,
                ScheduledAt = order.ScheduledAt,
                Location = order.Location,
                Notes = order.Notes
            };
        }

        public async Task<List<OrderResponseDto>> GetMyOrdersAsync(int customerId)
        {
            var orders = await _orderRepo.GetByCustomerAsync(customerId);

            return orders.Select(o => MapToOrderResponseDto(o)).ToList();
        }

        public async Task<OrderResponseDto> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);

            if (order == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }

            return MapToOrderResponseDto(order);
        }

        public async Task CancelOrderAsync(int orderId, int customerId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);

            if (order == null || order.CustomerId != customerId)
            {
                throw new KeyNotFoundException("Order not found.");
            }

            if (order.Status != Enums.OrderStatus.Pending)
            {
                throw new InvalidOperationException("Order cannot be cancelled.");
            }

            order.Status = Enums.OrderStatus.Cancelled;
            await _orderRepo.UpdateAsync(order);
            var body = $"Booking Cancelled\nOrder ID: {order.OrderId}";
            await _emailService.SendEmailAsync(order.Customer.Email,"GreenCarWash Booking Cancelled",body); 
        }

        public async Task<Car> AddCarAsync(int customerId, AddCarRequestDto request)
        {
            var existingCar = await _carRepo.GetByLicensePlateAsync(request.LicensePlate);

            if (existingCar != null)
            {
                throw new InvalidOperationException("A car with this license plate already exists.");
            }
            var car = new Car
            {
                CustomerId = customerId,
                Make = request.Make,
                Model = request.Model,
                Year = request.Year,
                LicensePlate = request.LicensePlate,
                ImageUrl = request.ImageUrl
            };

            return await _carRepo.AddAsync(car);
        }

        public async Task<CatalogResponseDto> GetCatalogAsync()
        {
            var plans = await _planRepo.GetAllActiveAsync();
            var addOns = await _addOnRepo.GetAllActiveAsync();

            return new CatalogResponseDto
            {
                Plans = plans.Select(p => new CatalogPlanDto
                {
                    PlanId = p.PlanId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    DurationMinutes = p.DurationMinutes
                }).ToList(),

                AddOns = addOns.Select(a => new CatalogAddOnDto
                {
                    AddOnId = a.AddOnId,
                    Name = a.Name,
                    Description = a.Description,
                    Price = a.Price
                }).ToList()
            };
        }
        
        private static OrderResponseDto MapToOrderResponseDto(Order o)
        {
            return new OrderResponseDto
            {
                OrderId = o.OrderId,
                Status = o.Status.ToString(),
                CustomerName = o.Customer?.Name ?? "",
                WasherName = o.Washer?.Name ?? "",
                CarDetails = o.Car != null
                    ? $"{o.Car.Make} {o.Car.Model} ({o.Car.Year}) - {o.Car.LicensePlate}"
                    : "",
                PlanName = o.ServicePlan?.Name ?? "",
                AddOnName = o.AddOn?.Name,
                AddOnPrice = o.AddOn?.Price ?? 0,
                TotalAmount = o.TotalAmount,
                ScheduledAt = o.ScheduledAt,
                Location = o.Location,
                Notes = o.Notes
            };
        }

        public async Task<CustomerProfileResponseDto> GetCustomerProfileAsync(int customerId)
        {
            var customer = await _userRepo.GetByIdAsync(customerId);
            if (customer == null)
            {
                throw new KeyNotFoundException("Customer not found");
            }
            return new CustomerProfileResponseDto
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                IsActive = customer.IsActive,
                CreatedAt = customer.CreatedAt
            };
        }

        public async Task UpdateCustomerProfileAsync(int customerId, UpdateCustomerRequestDto request)
        {
            var customer = await _userRepo.GetByIdAsync(customerId);
            if (customer == null)
            {
                throw new KeyNotFoundException("Customer not found");
            }
            customer.Name = request.Name;
            customer.Phone = request.Phone;
            customer.IsActive = request.IsActive;
            await _userRepo.UpdateAsync(customer);
        }
    }
}