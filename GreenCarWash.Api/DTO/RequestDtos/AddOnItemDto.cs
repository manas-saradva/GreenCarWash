using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.DTOs.RequestDtos
{
    public class AddOnItemDto
    {
        [Required]
        public int AddOnId{get;set;}

        [Required]
        [Range(1,100)]
        public int Qty{get;set;}
    }
}