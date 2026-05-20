using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GreenCarWash.Api.Models;

namespace GreenCarWash.Api.Interfaces
{
    public interface IServicePlanRepository
    {
        Task<List<ServicePlan>> GetAllActiveAsync();
        Task<ServicePlan?> GetByIdAsync(int id);
        Task<ServicePlan> AddAsync(ServicePlan plan);
        Task<ServicePlan> UpdateAsync(ServicePlan plan);
    }
}