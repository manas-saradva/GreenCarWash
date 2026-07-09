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
        Task<Car?> GetByLicensePlateAsync(string licensePlate);
    }
}