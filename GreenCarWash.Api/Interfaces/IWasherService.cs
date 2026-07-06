using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenCarWash.Api.DTOs.RequestDtos;
using GreenCarWash.Api.DTOs.ResponseDtos;
using GreenCarWash.Api.Models;

namespace GreenCarWash.Api.Interfaces
{
    public interface IWasherService
    {
        Task AcceptOrderAsync(int orderId, int washerId);
        Task DeclineOrderAsync(int orderId, int washerId);
        Task StartOrderAsync(int orderId, int washerId);
        Task CompleteOrderAsync(int orderId, int washerId);
        Task<List<OrderResponseDto>> GetMyOrdersAsync(int washerId);
        Task<WasherProfileResponseDto> GetProfileAsync(int washerId);
        Task UpdateProfileAsync(int washerId, UpdateWasherRequestDto dto);
    }
}