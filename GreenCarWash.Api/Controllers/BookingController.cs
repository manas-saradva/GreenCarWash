using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenCarWash.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GreenCarWash.Api.DTOs.RequestDtos;

namespace GreenCarWash.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        private int GetCustomerId(){
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        }

        [HttpPost("book")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Book(BookingRequestDto request)
        {
            var result = await _bookingService.BookAsync(GetCustomerId(), request);
            return Ok(result);
        }

        [HttpGet("my-orders")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyOrders()
        {
            var result = await _bookingService.GetMyOrdersAsync(GetCustomerId());
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var result = await _bookingService.GetOrderByIdAsync(id);
            return Ok(result);
        }

        [HttpDelete("{id}/cancel")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            await _bookingService.CancelOrderAsync(id, GetCustomerId());
            return Ok(new { Message = "Order cancelled successfully" });
        }

        [HttpPost("cars")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddCar(AddCarRequestDto request)
        {
            var result = await _bookingService.AddCarAsync(GetCustomerId(), request);
            return Ok(result);
        }

        [HttpGet("catalog")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCatalog()
        {
            var result = await _bookingService.GetCatalogAsync();
            return Ok(result);
        }
    }
}