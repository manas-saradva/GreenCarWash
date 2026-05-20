using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.Models
{
    public class ServicePlan
    {
        [Key]
        public int PlanId{get;set;}

        [Required]
        [StringLength(100)]
        public string Name{get;set;} = string.Empty;

        [StringLength(500)]
        public string Description{get;set;} = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price{get;set;}

        public int DurationMinutes{get;set;}

        public bool IsActive{get;set;} = true;

        public ICollection<Order> Order {get;set;} = new List<Order>();
    }
}