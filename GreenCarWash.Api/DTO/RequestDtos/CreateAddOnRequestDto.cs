using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.DTOs.RequestDtos
{
    public class CreateAddOnRequestDto
    {
        [Required]
        [StringLength(100)]
        public string Name{get;set;} = string.Empty;

        [StringLength(500)]
        public string Description{get;set;} = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price{get;set;}

        [Required]
        public bool IsActive{get;set;} = true;
    }
}