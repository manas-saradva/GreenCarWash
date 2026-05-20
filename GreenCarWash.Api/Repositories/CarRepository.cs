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
    public class CarRepository : ICarRepository
    {
        private readonly CarWashDbContext _context;

        public CarRepository(CarWashDbContext context)
        {
            _context = context;
        }

        public async Task<Car> AddAsync(Car car)
        {
            await _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();
            return car;
        }

        public async Task<Car?> GetByIdAsync(int id)
        {
            return await _context.Cars.FindAsync(id);
        }

        public async Task<List<Car>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Cars.Where(c => c.CustomerId == customerId).ToListAsync();
        }

        public async Task<bool> BelongsToCustomerAsync(int carId, int customerId)
        {
            return await _context.Cars.AnyAsync(c => c.CarId == carId && c.CustomerId == customerId);
        }

        public async Task<Car?> GetByLicensePlateAsync(string licensePlate)
        {
            return await _context.Cars.FirstOrDefaultAsync(c => c.LicensePlate == licensePlate);
        }
    }
}