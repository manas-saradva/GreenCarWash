using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.DTOs.RequestDtos
{
    public class UpdateWasherRequestDto
    {
        [Required]
        [StringLength(100)]
        public string Name{get;set;} = string.Empty;

        [Required]
        [StringLength(20)]
        public string Phone{get;set;} = string.Empty;
    }
}