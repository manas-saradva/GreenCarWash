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
    public class ServicePlanRepository : IServicePlanRepository
    {
        private readonly CarWashDbContext _context;

        public ServicePlanRepository(CarWashDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServicePlan>> GetAllActiveAsync()
        {
            return await _context.ServicePlans.Where(p => p.IsActive).ToListAsync();
        }

        public async Task<ServicePlan?> GetByIdAsync(int id)
        {
            return await _context.ServicePlans.FindAsync(id);
        }

        public async Task<ServicePlan> AddAsync(ServicePlan plan)
        {
            await _context.ServicePlans.AddAsync(plan);
            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task<ServicePlan> UpdateAsync(ServicePlan plan)
        {
            _context.ServicePlans.Update(plan);
            await _context.SaveChangesAsync();
            return plan;
        }
    }
}