using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenCarWash.Api.DTOs.RequestDtos;
using GreenCarWash.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GreenCarWash.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Washer")]
    public class WasherController : ControllerBase
    {
        private readonly IWasherService _washerService;

        public WasherController(IWasherService washerService)
        {
            _washerService = washerService;
        }

        private int GetWasherId(){
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        }

        [HttpPost("orders/{id}/accept")]
        public async Task<IActionResult> AcceptOrder(int id)
        {
            await _washerService.AcceptOrderAsync(id, GetWasherId());
            return Ok("Order Accepted");
        }

        [HttpPost("orders/{id}/decline")]
        public async Task<IActionResult> DeclineOrder(int id)
        {
            await _washerService.DeclineOrderAsync(id, GetWasherId());
            return Ok("Order Declined");
        }

        [HttpPost("orders/{id}/start")]
        public async Task<IActionResult> StartOrder(int id)
        {
            await _washerService.StartOrderAsync(id, GetWasherId());
            return Ok("Order Started");
        }

        [HttpPost("orders/{id}/complete")]
        public async Task<IActionResult> CompleteOrder(int id)
        {
            await _washerService.CompleteOrderAsync(id, GetWasherId());
            return Ok("Order Completed");
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var result = await _washerService.GetMyOrdersAsync(GetWasherId());
            return Ok(result);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var result = await _washerService.GetProfileAsync(GetWasherId());
            return Ok(result);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateWasherRequestDto request)
        {
            await _washerService.UpdateProfileAsync(GetWasherId(), request);
            return Ok("Profile updated successfully");
        }
    }
}