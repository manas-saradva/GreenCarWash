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
    public class WasherRepository : IWasherRepository
    {
        private readonly CarWashDbContext _context;

        public WasherRepository(CarWashDbContext context)
        {
            _context = context;
        }

        public async Task<Washer?> FindByEmailAsync(string email)
        {
            return await _context.Washers.FirstOrDefaultAsync(w => w.Email == email);
        }

        public async Task<Washer?> GetByIdAsync(int id)
        {
            return await _context.Washers.FindAsync(id);
        }

        // public async Task<List<Washer>> GetAllActiveAsync()
        // {
        //     return await _context.Washers.Where(w => w.IsActive).ToListAsync();
        // }

        public async Task<Washer> AddAsync(Washer washer)
        {
            await _context.Washers.AddAsync(washer);
            await _context.SaveChangesAsync();
            return washer;
        }

        public async  Task<Washer> UpdateAsync(Washer washer)
        {
            _context.Washers.Update(washer);
            await _context.SaveChangesAsync();
            return washer;
        }
    }
}