using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
            var addOnLines = new List<OrderAddOnDto>();

            if(request.AddOns != null && request.AddOns.Any())
            {
                //var addOnIds = request.AddOns.Select(a => a.AddOnId).ToList();
                var addOnIds = request.AddOns;

                if (addOnIds.Distinct().Count() != addOnIds.Count)
                {
                    throw new ArgumentException("Cannot add the same AddOn multiple times.");
                }

                var addOns = await _addOnRepo.GetByIdsAsync(addOnIds);

                foreach(var addOnId in addOnIds)
                {
                    var addOn = addOns.FirstOrDefault(a => a.AddOnId == addOnId);

                    if(addOn != null && addOn.IsActive)
                    {
                        totalAmount += addOn.Price;

                        addOnLines.Add(new OrderAddOnDto
                        {
                           AddOnName = addOn.Name,
                           Price = addOn.Price
                        });
                    }
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
                AddOnsJson = JsonSerializer.Serialize(request.AddOns ?? new List<int>()),
                Status = Enums.OrderStatus.Pending,
                ScheduledAt = request.ScheduledAt,
                Location = request.Location,
                Notes = request.Notes,
                TotalAmount = totalAmount,
                CreatedAt = DateTime.UtcNow
            };

            await _orderRepo.AddAsync(order);


            var customer = await _userRepo.GetByIdAsync(customerId);

            if(customer != null)
            {
                var body = $"""
                <h2>Booking Confirmed</h2>
                <p>Order ID: {order.OrderId}</p>
                <p>Service Plan: {plan.Name}</p>
                <p>Scheduled At: {order.ScheduledAt}</p>
                <p>Location: {order.Location}</p>
                <p>Total Amount: ₹{order.TotalAmount}</p>
                """;

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
                AddOn = addOnLines,
                TotalAmount = order.TotalAmount,
                ScheduledAt = order.ScheduledAt,
                Location = order.Location,
                Notes = order.Notes,
                PaymentMethod = order.PaymentMethod?.ToString() ?? "",
                PaymentStatus = order.PaymentStatus?.ToString() ?? "",
                CreatedAt = order.CreatedAt
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

            var responseDto = MapToOrderResponseDto(order);
            
            // Map the AddOn names correctly
            if (responseDto.AddOn != null && responseDto.AddOn.Any()) 
            {
                var addOnIds = JsonSerializer.Deserialize<List<AddOnItemDto>>(order.AddOnsJson ?? "[]")?.Select(a => a.AddOnId).ToList() ?? new List<int>();
                if (addOnIds.Any())
                {
                    var addOnsData = await _addOnRepo.GetByIdsAsync(addOnIds);
                    foreach (var line in responseDto.AddOn)
                    {
                        var matchingData = addOnsData.FirstOrDefault(a => a.AddOnId == addOnIds[responseDto.AddOn.IndexOf(line)]);
                        if (matchingData != null)
                        {
                            line.AddOnName = matchingData.Name;
                            line.Price = matchingData.Price;
                        }
                    }
                }
            }
            return responseDto;
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
            var addOnLines = new List<OrderAddOnDto>();
            try
            {
                var items = JsonSerializer.Deserialize<List<AddOnItemDto>>(o.AddOnsJson ?? "[]");
                if(items != null)
                {
                    foreach(var item in items)
                    {
                        addOnLines.Add(new OrderAddOnDto
                        {
                            AddOnName = "",
                            Price = 0
                        });
                    }
                }
            }
            catch
            {}

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
                AddOn = addOnLines,
                TotalAmount = o.TotalAmount,
                ScheduledAt = o.ScheduledAt,
                Location = o.Location,
                Notes = o.Notes,
                PaymentMethod = o.PaymentMethod?.ToString() ?? "",
                PaymentStatus = o.PaymentStatus?.ToString() ?? "",
                CreatedAt = o.CreatedAt
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