using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.DTOs.ResponseDtos
{
    public class ApiErrorResponseDto
    {
        public string Message{get;set;} = string.Empty;
        public int StatusCode{get;set;}
        public DateTime Timestamp{get;set;} = DateTime.UtcNow;
    }
}