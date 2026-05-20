using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GreenCarWash.Api.Models;
using Microsoft.AspNetCore.DataProtection.Repositories;

namespace GreenCarWash.Api.Interfaces
{
    public interface IAddOnRepository
    {
        Task<List<Add_on>> GetAllActiveAsync();
        Task<Add_on?> GetByIdAsync(int id);
        Task<List<Add_on>> GetByIdsAsync(List<int> ids);
        Task<Add_on> AddAsync(Add_on entity);
        Task UpdateAsync(Add_on entity);
    }
}