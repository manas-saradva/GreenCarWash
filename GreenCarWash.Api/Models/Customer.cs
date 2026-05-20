using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenCarWash.Api.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId{get;set;}

        [Required]
        [StringLength(100)]
        public string Name{get;set;} = string.Empty;

        [Required]
        [StringLength(150)]
        public string Email{get;set;} = string.Empty;

        [Required]
        [StringLength(255)]
        public string PasswordHash{get;set;} = string.Empty;

        [Required]
        [StringLength(20)]
        public string Phone{get;set;} = string.Empty;

        public bool IsActive{get;set;} = true;

        public DateTime CreatedAt{get;set;} = DateTime.UtcNow;

        public ICollection<Car> Car{get;set;} = new List<Car>();
        public ICollection<Order> Order{get;set;} = new List<Order>();
        public ICollection<Review> Review{get;set;} = new List<Review>();
    }
}