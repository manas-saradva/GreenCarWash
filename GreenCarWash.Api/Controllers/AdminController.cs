using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenCarWash.Api.DTOs.RequestDtos;
using GreenCarWash.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenCarWash.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers()
        {
            return Ok(await _adminService.GetCustomersAsync());
        }

        [HttpGet("washers")]
        public async Task<IActionResult> GetWashers(){
            return Ok(await _adminService.GetWashersAsync());
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders(){
            return Ok(await _adminService.GetOrdersAsync());
        }

        [HttpPost("orders/{id}/assign")]
        public async Task<IActionResult> AssignWasher(int id,AssignWasherRequestDto request)
        {
            await _adminService.AssignWasherAsync(id, request.WasherId);
            return Ok("Washer Assigned");
        }

        [HttpGet("reports")]
        public async Task<IActionResult> GetReports(){
            return Ok(await _adminService.GetReportsAsync());
        }

        [HttpPost("promo-codes")]
        public async Task<IActionResult> CreatePromoCode(PromoCodeRequestDto request)
        {
            await _adminService.CreatePromoCodeAsync(request);
            return Ok("Promo code created");
        }

        [HttpPut("promo-codes/{id}")]
        public async Task<IActionResult> UpdatePromoCode(int id,PromoCodeRequestDto request)
        {
            await _adminService.UpdatePromoCodeAsync(id, request);
            return Ok("Promo code updated successfully");
        }

        [HttpPost("plans")]
        public async Task<IActionResult> CreateServicePlan(CreatePlanRequestDto request)
        {
            await _adminService.CreateServicePlanAsync(request);
            return Ok("New service plan created");
        }

        [HttpPut("plans/{id}")]
        public async Task<IActionResult> UpdateServicePlan(int id,CreatePlanRequestDto request)
        {
            return Ok(await _adminService.UpdateServicePlanAsync(id, request));
        }

        [HttpPost("addons")]
        public async Task<IActionResult> CreateAddOn(CreateAddOnRequestDto request)
        {
            await _adminService.CreateAddOnAsync(request);
            return Ok("New add-on created");
        }

        [HttpPut("addons/{id}")]
        public async Task<IActionResult> UpdateAddOn(int id,CreateAddOnRequestDto request)
        {
            return Ok(await _adminService.UpdateAddOnAsync(id, request));
        }
    }
}