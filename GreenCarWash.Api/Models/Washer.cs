using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.Models
{
    public class Washer
    {
        [Key]
        public int WasherId{get;set;}

        [Required]
        [StringLength(100)]
        public string Name {get;set;} = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email{get;set;} = string.Empty;

        [Required]
        [StringLength(255)]
        public string PasswordHash{get;set;} = string.Empty;

        [Required]
        [StringLength(20)]
        public string Phone{get;set;} = string.Empty;

        public bool IsActive{get;set;} = true;

        [Column(TypeName = "decimal(3,2)")]
        public decimal AverageRating{get;set;} = 0;

        public ICollection<Order> Order{get;set;} = new List<Order>();
        public ICollection<Review> Review{get;set;} = new List<Review>();
    }
}