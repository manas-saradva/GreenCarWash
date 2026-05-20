using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.DTOs.ResponseDtos
{
    public class OrderResponseDto
    {
        public int OrderId{get;set;}
        public string Status{get;set;} = string.Empty;
        public string CustomerName{get;set;} = string.Empty;
        public string WasherName{get;set;} = string.Empty;
        public string CarDetails{get;set;} = string.Empty;
        public string PlanName{get;set;} = string.Empty;
        public List<OrderAddOnDto> AddOn {get;set;} = new List<OrderAddOnDto>();
        public decimal TotalAmount{get;set;}
        public DateTime ScheduledAt{get;set;}
        public string Location{get;set;} = string.Empty;
        public string Notes{get;set;} = string.Empty;
        public string PaymentMethod{get;set;} = string.Empty;
        public string PaymentStatus{get;set;} = string.Empty;
        public DateTime CreatedAt{get;set;}
    }
}