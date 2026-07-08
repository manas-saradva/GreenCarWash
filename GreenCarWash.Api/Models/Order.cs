using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using GreenCarWash.Api.Enums;

namespace GreenCarWash.Api.Models
{
    public class Order
    {
        [Key]
        public int OrderId{get;set;}

        public int CustomerId{get;set;}
        [ForeignKey("CustomerId")]
        public Customer? Customer{get;set;}

        public int? WasherId{get;set;}
        [ForeignKey("WasherId")]
        public Washer? Washer{get;set;}

        public int CarId{get;set;}
        [ForeignKey("CarId")]
        public Car? Car{get;set;}

        public int PlanId{get;set;}
        [ForeignKey("PlanId")]
        public ServicePlan? ServicePlan{get;set;}

        public int? PromoCodeId{get;set;}
        [ForeignKey("PromoCodeId")]
        public Promo_code? PromoCode{get;set;}

        public string AddOnsJson{get;set;} = "[]";

        public OrderStatus Status{get;set;} = OrderStatus.Pending;

        public DateTime ScheduledAt{get;set;}

        [StringLength(300)]
        public string Location{get;set;} = string.Empty;

        [StringLength(500)]
        public string Notes{get;set;} = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount{get;set;}

        public Review? Review{get;set;}
    }
}