using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.DTOs.ResponseDtos
{
    public class CustomerProfileResponseDto
    {
        public int CustomerId{get;set;}
        public string Name{get;set;} = string.Empty;
        public string Email{get;set;} = string.Empty;
        public string Phone{get;set;} = string.Empty;
        public bool IsActive{get;set;}
        public DateTime CreatedAt{get;set;}
    }
}
