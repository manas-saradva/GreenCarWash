using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.DTOs.RequestDtos
{
    public class AddCarRequestDto
    {
        [Required]
        [StringLength(50)]
        public string Make{get;set;} = string.Empty;

        [Required]
        [StringLength(50)]
        public string Model{get;set;} = string.Empty;

        [Required]
        public int Year{get;set;}

        [Required]
        [StringLength(20)]
        public string LicensePlate{get;set;} = string.Empty;

        [StringLength(500)]
        public string ImageUrl{get;set;} = string.Empty;
    }
}