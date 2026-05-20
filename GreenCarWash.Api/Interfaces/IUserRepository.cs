using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GreenCarWash.Api.Models;

namespace GreenCarWash.Api.Interfaces
{
    public interface IUserRepository
    {
        Task<Customer?> FindByEmailAsync(string email);
        Task<Customer?> GetByIdAsync(int id);
        Task<Customer> AddAsync(Customer customer);
        Task<bool> ExistsAsync(string email);
        Task<Customer> UpdateAsync(Customer customer);
    }
}