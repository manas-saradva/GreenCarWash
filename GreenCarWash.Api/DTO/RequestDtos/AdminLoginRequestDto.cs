using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.DTOs.RequestDtos
{
    public class AdminLoginRequestDto
    {
        [Required]
        [StringLength(100)]
        public string Username{get;set;} = string.Empty;

        [Required]
        public string Password{get;set;} = string.Empty;
    }
}