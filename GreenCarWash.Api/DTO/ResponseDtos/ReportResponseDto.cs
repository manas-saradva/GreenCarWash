using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.DTOs.ResponseDtos
{
    public class ReportResponseDto
    {
        public int TotalOrders{get;set;}
        public decimal TotalRevenue{get;set;}
        public int CompletedOrders{get;set;}
        public int CancelledOrders{get;set;}
        public int PendingOrders{get;set;}
    }
}