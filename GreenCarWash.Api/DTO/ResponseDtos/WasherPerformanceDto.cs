using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.DTOs.ResponseDtos
{
    public class WasherPerformanceDto
    {
        public int WasherId{get;set;}
        public string Name{get;set;} = string.Empty;
        public int CompletedOrders{get;set;}
        public decimal AverageRating{get;set;}
    }
}