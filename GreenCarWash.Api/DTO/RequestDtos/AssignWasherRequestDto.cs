using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.DTOs.RequestDtos
{
    public class AssignWasherRequestDto
    {
        [Required]
        public int WasherId{get;set;}
    }
}