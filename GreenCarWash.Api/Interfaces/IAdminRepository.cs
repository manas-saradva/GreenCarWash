using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GreenCarWash.Api.Models;

namespace GreenCarWash.Api.Interfaces
{
    public interface IAdminRepository
    {
        Task<Admin?> GetByUsernameAsync(string username);
        Task<List<Customer>> GetAllCustomersAsync();
        Task<List<Washer>> GetAllWashersAsync();
        Task<List<Order>> GetAllOrdersAsync();
        
    }
}