using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GreenCarWash.Api.Models;

namespace GreenCarWash.Api.Interfaces
{
    public interface IReviewRepository
    {
        Task AddAsync(Review review);
        Task<Review?> GetByOrderIdAsync(int orderId);
        Task<List<Review>> GetByWasherIdAsync(int washerId);
        Task<double> GetAverageRatingAsync(int washerId);
    }
}