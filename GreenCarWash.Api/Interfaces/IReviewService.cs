using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenCarWash.Api.DTOs.RequestDtos;
using GreenCarWash.Api.DTOs.ResponseDtos;

namespace GreenCarWash.Api.Interfaces
{
    public interface IReviewService
    {
        Task AddReviewAsync(int customerId, ReviewRequestDto dto);
        Task<List<ReviewResponseDto>> GetWasherReviewsAsync(int washerId);
    }
}