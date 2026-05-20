using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.DTOs.ResponseDtos
{
    public class AuthResponseDto
    {
        public string Token{get;set;} = string.Empty;
        public string Role{get;set;} = string.Empty;
        public int UserId{get;set;}
        public DateTime ExpiresAt{get;set;}
    }
}