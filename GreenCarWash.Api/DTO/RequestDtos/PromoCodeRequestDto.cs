using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.DTOs.RequestDtos
{
    public class PromoCodeRequestDto
    {
        [Required]
        [StringLength(50)]
        public string Code{get;set;} = string.Empty;

        [Required]
        [Range(0,100)]
        [Column(TypeName = "decimal(5,2)")]
        public decimal DiscountPercent{get;set;}

        [Required]
        public DateTime ExpiryDate{get;set;}

        [Required]
        public int MaxUses{get;set;}

        public bool IsActive{get;set;} = true;
    }
}