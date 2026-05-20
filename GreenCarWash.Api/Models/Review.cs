using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.Models
{
    public class Review
    {
        [Key]
        public int ReviewId{get;set;}

        public int OrderId{get;set;}
        [ForeignKey("OrderId")]
        public Order? Order{get;set;}

        public int CustomerId{get;set;}
        [ForeignKey("CustomerId")]
        public Customer? Customer{get;set;}

        public int WasherId{get;set;}
        [ForeignKey("WasherId")]
        public Washer? Washer{get;set;}

        [Range(1,5)]
        public int Rating{get;set;}

        [StringLength(1000)]
        public string Comment{get;set;} = string.Empty;

        public DateTime CreatedAt{get;set;} = DateTime.UtcNow;
    }
}