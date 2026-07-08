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
    public class OrderRepository : IOrderRepository
    {
        private readonly CarWashDbContext _context;

        public OrderRepository(CarWashDbContext context)
        {
            _context = context;
        }

        // public async Task<List<Order>> GetAllAsync()
        // {
        //     return await _context.Orders
        //         .Include(o => o.Car)
        //         .Include(o => o.ServicePlan)
        //         .Include(o => o.Customer)
        //         .Include(o => o.Washer)
        //         .ToListAsync();
        // }

        public async Task<List<Order>> GetByCustomerAsync(int customerId)
        {
            return await _context.Orders
                .Include(o => o.Car)
                .Include(o => o.ServicePlan)
                .Include(o => o.Customer)
                .Include(o => o.Washer)
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<List<Order>> GetByWasherAsync(int washerId)
        {
            return await _context.Orders
                .Include(o => o.Car)
                .Include(o => o.ServicePlan)
                .Include(o => o.Customer)
                .Include(o => o.Washer)
                .Where(o => o.WasherId == washerId)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Car)
                .Include(o => o.ServicePlan)
                .Include(o => o.Customer)
                .Include(o => o.Washer)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task<Order> AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        // public async Task<int> CountPromoUsageAsync(int promoCodeId)
        // {
        //     return await _context.Orders.CountAsync(o => o.PromoCodeId == promoCodeId);
        // }
    }
}