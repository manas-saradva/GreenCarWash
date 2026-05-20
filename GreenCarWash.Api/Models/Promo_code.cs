using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.Models
{
    public class Promo_code
    {
        [Key]
        public int PromoCodeId{get;set;}

        [Required]
        [StringLength(50)]
        public string Code{get;set;} = string.Empty;

        [Range(0,100)]
        [Column(TypeName = "decimal(5,2)")]
        public decimal DiscountPercent{get;set;}

        public DateTime ExpiryDate{get;set;}

        public int MaxUses{get;set;}

        public bool IsActive{get;set;} = true;

        public ICollection<Order> Order{get;set;} = new List<Order>();
    }
}