using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.DTOs.RequestDtos
{
    public class RegisterRequestDto
    {
        [Required]
        [StringLength(100)]
        public string Name{get;set;} = string.Empty;

        [Required]
        [StringLength(150)]
        public string Email{get;set;} = string.Empty;

        [Required]
        [StringLength(20)]
        public string Phone{get;set;} = string.Empty;

        [Required]
        [StringLength(256)]
        public string Password{get;set;} = string.Empty;

        [Required]
        public string Role{get;set;} = string.Empty;
    }
}