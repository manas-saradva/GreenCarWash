using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.DTOs.ResponseDtos
{
    public class OrderAddOnDto
    {
        public string AddOnName{get;set;} = string.Empty;
        public decimal Price{get;set;}
    }
}