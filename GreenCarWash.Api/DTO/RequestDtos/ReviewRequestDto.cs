using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.DTOs.RequestDtos
{
    public class ReviewRequestDto
    {
        [Required]
        public int OrderId{get;set;}

        [Required]
        public int WasherId{get;set;}

        [Required]
        [Range(1,5)]
        public int Rating{get;set;}

        [StringLength(1000)]
        public string Comment{get;set;} = string.Empty;
    }
}