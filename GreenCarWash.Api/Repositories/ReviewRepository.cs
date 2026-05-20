using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenCarWash.Api.Data;
using GreenCarWash.Api.Interfaces;
using GreenCarWash.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GreenCarWash.Api.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly CarWashDbContext _context;

        public ReviewRepository(CarWashDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task<Review?> GetByOrderIdAsync(int orderId)
        {
            return await _context.Reviews.FirstOrDefaultAsync(r => r.OrderId == orderId);
        }

        public async Task<List<Review>> GetByWasherIdAsync(int washerId)
        {
            return await _context.Reviews
                .Include(r => r.Customer)
                .Where(r => r.WasherId == washerId)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync(int washerId)
        {
            var avg = await _context.Reviews.Where(r => r.WasherId == washerId).AverageAsync(r => (double?)r.Rating);
            return avg ?? 0;
        }
    }
}