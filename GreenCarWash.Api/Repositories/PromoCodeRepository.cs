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
    public class PromoCodeRepository : IPromoCodeRepository
    {
        private readonly CarWashDbContext _context;

        public PromoCodeRepository(CarWashDbContext context)
        {
            _context = context;
        }

        public async Task<Promo_code?> GetByIdAsync(int id)
        {
            return await _context.PromoCodes.FindAsync(id);
        }

        public async Task<Promo_code?> GetByCodeAsync(string code)
        {
            return await _context.PromoCodes.FirstOrDefaultAsync(p => p.Code == code);
        }

        public async Task AddAsync(Promo_code promo)
        {
            await _context.PromoCodes.AddAsync(promo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Promo_code promo)
        {
            _context.PromoCodes.Update(promo);
            await _context.SaveChangesAsync();
        }
    }
}