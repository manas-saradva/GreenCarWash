using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenCarWash.Api.DTOs.RequestDtos;
using GreenCarWash.Api.DTOs.ResponseDtos;
using GreenCarWash.Api.Models;

namespace GreenCarWash.Api.Interfaces
{
    public interface IBookingService
    {
        Task<OrderResponseDto> BookAsync(int customerId, BookingRequestDto dto);
        Task<List<OrderResponseDto>> GetMyOrdersAsync(int customerId);
        Task<OrderResponseDto> GetOrderByIdAsync(int orderId);
        Task CancelOrderAsync(int orderId, int customerId);
        Task<Car> AddCarAsync(int customerId, AddCarRequestDto dto);
        Task<CatalogResponseDto> GetCatalogAsync();
        Task<CustomerProfileResponseDto> GetCustomerProfileAsync(int customerId);
        Task UpdateCustomerProfileAsync(int customerId, UpdateCustomerRequestDto dto);
    }
}