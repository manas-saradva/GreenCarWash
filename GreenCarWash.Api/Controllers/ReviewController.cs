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
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddReview(ReviewRequestDto request)
        {
            var customerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            await _reviewService.AddReviewAsync(customerId, request);
            return Ok("Review added successfully");
        }

        [HttpGet("washer/{washerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetWasherReviews(int washerId)
        {
            var result = await _reviewService.GetWasherReviewsAsync(washerId);
            return Ok(result);
        }
    }
}