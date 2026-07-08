using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GreenCarWash.Api.Models;

namespace GreenCarWash.Api.Interfaces
{
    public interface IWasherRepository
    {
        Task<Washer?> FindByEmailAsync(string email);
        Task<Washer?> GetByIdAsync(int id);
        //Task<List<Washer>> GetAllActiveAsync();
        Task<Washer> AddAsync(Washer washer);
        Task<Washer> UpdateAsync(Washer washer);
    }
}