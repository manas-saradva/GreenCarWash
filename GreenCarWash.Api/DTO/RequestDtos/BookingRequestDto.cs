using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.DTOs.RequestDtos
{
    public class BookingRequestDto
    {
        [Required]
        public int CarId{get;set;}

        [Required]
        public int PlanId{get;set;}

        [Required]
        [StringLength(300)]
        public string Location{get;set;} = string.Empty;

        [StringLength(500)]
        public string Notes{get;set;} = string.Empty;

        public int? AddOnId{get;set;}

        public string? PromoCode{get;set;}

        [Required]
        public DateTime ScheduledAt{get;set;}
    }
}