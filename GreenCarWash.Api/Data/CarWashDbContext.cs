using GreenCarWash.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GreenCarWash.Api.Data
{
    public class CarWashDbContext : DbContext
    {
        public CarWashDbContext(DbContextOptions<CarWashDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Washer> Washers { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<ServicePlan> ServicePlans { get; set; }
        public DbSet<Add_on> AddOns { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Promo_code> PromoCodes { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Unique columns
            modelBuilder.Entity<Customer>().HasIndex(c => c.Email).IsUnique();
            modelBuilder.Entity<Washer>().HasIndex(w => w.Email).IsUnique();
            modelBuilder.Entity<Car>().HasIndex(c => c.LicensePlate).IsUnique();
            modelBuilder.Entity<Promo_code>().HasIndex(p => p.Code).IsUnique();
            modelBuilder.Entity<Review>().HasIndex(r => r.OrderId).IsUnique();


            //Defining relationships between tables

            //Orders Table
            //Order -> Customer
            modelBuilder.Entity<Order>().HasOne(o => o.Customer).WithMany(o => o.Order).HasForeignKey(o => o.CustomerId).OnDelete(DeleteBehavior.Restrict);

            //Order -> Washer
            modelBuilder.Entity<Order>().HasOne(w => w.Washer).WithMany(w => w.Order).HasForeignKey(w => w.WasherId).OnDelete(DeleteBehavior.SetNull);

            //Order -> Car
            modelBuilder.Entity<Order>().HasOne(c => c.Car).WithMany().HasForeignKey(c => c.CarId).OnDelete(DeleteBehavior.Restrict);

            //Order -> Service_Plan
            modelBuilder.Entity<Order>().HasOne(s => s.ServicePlan).WithMany(s => s.Order).HasForeignKey(s => s.PlanId).OnDelete(DeleteBehavior.Restrict);

            //Order -> Promo_Code
            modelBuilder.Entity<Order>().HasOne(p => p.PromoCode).WithMany(p => p.Order).HasForeignKey(p => p.PromoCodeId).OnDelete(DeleteBehavior.SetNull);


            //Reviews Table
            //Review -> Order
            modelBuilder.Entity<Review>().HasOne(o => o.Order).WithOne(o => o.Review).HasForeignKey<Review>(o => o.OrderId).OnDelete(DeleteBehavior.Restrict);

            //Review -> Customer
            modelBuilder.Entity<Review>().HasOne(c => c.Customer).WithMany(c => c.Review).HasForeignKey(c => c.CustomerId).OnDelete(DeleteBehavior.Restrict);

            //Review -> Washer
            modelBuilder.Entity<Review>().HasOne(w => w.Washer).WithMany(w => w.Review).HasForeignKey(w => w.WasherId).OnDelete(DeleteBehavior.Restrict);

            //Cars Table
            //Car -> Customer
            modelBuilder.Entity<Car>().HasOne(c => c.Customer).WithMany(c => c.Car).HasForeignKey(c => c.CustomerId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}