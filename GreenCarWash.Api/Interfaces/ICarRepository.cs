using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GreenCarWash.Api.Models;

namespace GreenCarWash.Api.Interfaces
{
    public interface ICarRepository
    {
        Task<Car> AddAsync(Car car);
        Task<Car?> GetByIdAsync(int id);
        Task<List<Car>> GetByCustomerIdAsync(int customerId);
        Task<bool> BelongsToCustomerAsync(int carId, int customerId);
        Task<Car?> GetByLicensePlateAsync(string licensePlate);
    }
}