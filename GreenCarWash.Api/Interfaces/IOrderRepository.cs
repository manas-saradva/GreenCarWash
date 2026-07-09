using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GreenCarWash.Api.Models;

namespace GreenCarWash.Api.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetByCustomerAsync(int customerId);
        Task<List<Order>> GetByWasherAsync(int washerId);
        Task<Order?> GetByIdAsync(int id);
        Task<Order> AddAsync(Order order);
        Task<Order> UpdateAsync(Order order);
    }
}