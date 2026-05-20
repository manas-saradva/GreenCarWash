using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenCarWash.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GreenCarWash.Api.Data
{
    public class DbSeeder
    {
        public static async Task SeedAdminAsync(CarWashDbContext context)
        {
            if(!await context.Admins.AnyAsync())
            {
                var admin = new Admin
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin")
                };

                await context.Admins.AddAsync(admin);
                await context.SaveChangesAsync();
            }
        }
    }
}