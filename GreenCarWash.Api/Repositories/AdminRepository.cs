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
    public class AdminRepository : IAdminRepository
    {
        private readonly CarWashDbContext _context;

        public AdminRepository(CarWashDbContext context)
        {
            _context = context;
        }

        public async Task<Admin?> GetByUsernameAsync(string username)
        {
            return await _context.Admins.FirstOrDefaultAsync(a => a.Username == username);
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<List<Washer>> GetAllWashersAsync()
        {
            return await _context.Washers.ToListAsync();
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Car)
                .Include(o => o.ServicePlan)
                .Include(o => o.Customer)
                .Include(o => o.Washer)
                .Include(o => o.AddOn)
                .ToListAsync();
        }
    }
}