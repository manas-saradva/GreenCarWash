using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GreenCarWash.Api.Data;
using GreenCarWash.Api.Interfaces;
using GreenCarWash.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GreenCarWash.Api.Repositories
{
    public class AddOnRepository : IAddOnRepository
    {
        private readonly CarWashDbContext _context;

        public AddOnRepository(CarWashDbContext context)
        {
            _context = context;
        }

        public async Task<List<Add_on>> GetAllActiveAsync()
        {
            return await _context.AddOns.Where(a => a.IsActive).ToListAsync();
        }

        public async Task<Add_on?> GetByIdAsync(int id)
        {
            return await _context.AddOns.FindAsync(id);
        }

        public async Task<Add_on> AddAsync(Add_on addOn)
        {
            await _context.AddOns.AddAsync(addOn);
            await _context.SaveChangesAsync();
            return addOn;
        }

        public async Task UpdateAsync(Add_on addOn)
        {
            _context.AddOns.Update(addOn);
            await _context.SaveChangesAsync();
        }
    }
}