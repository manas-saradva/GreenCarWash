using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.DTOs.ResponseDtos
{
    public class ReviewResponseDto
    {
        public int ReviewId{get;set;}
        public int Rating{get;set;}
        public string Comment{get;set;} = string.Empty;
        public string CustomerName{get;set;} = string.Empty;
        public DateTime CreatedAt{get;set;}
    }
}