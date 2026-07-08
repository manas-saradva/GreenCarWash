using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenCarWash.Api.DTOs.RequestDtos;
using GreenCarWash.Api.DTOs.ResponseDtos;
using GreenCarWash.Api.Models;

namespace GreenCarWash.Api.Interfaces
{
    public interface IAdminService
    {
        Task<List<Customer>> GetCustomersAsync();
        Task<List<WasherProfileResponseDto>> GetWashersAsync();
        Task<List<OrderResponseDto>> GetOrdersAsync();
        Task AssignWasherAsync(int orderId, int washerId);
        Task<ReportResponseDto> GetReportsAsync();
        Task CreatePromoCodeAsync(PromoCodeRequestDto dto);
        Task UpdatePromoCodeAsync(int id, PromoCodeRequestDto dto);
        Task CreateServicePlanAsync(CreatePlanRequestDto dto);
        Task<ServicePlan> UpdateServicePlanAsync(int id, CreatePlanRequestDto dto);
        Task CreateAddOnAsync(CreateAddOnRequestDto dto);
        Task<Add_on> UpdateAddOnAsync(int id, CreateAddOnRequestDto dto);
    }
}