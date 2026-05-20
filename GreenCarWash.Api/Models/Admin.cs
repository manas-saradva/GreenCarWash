using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.Models
{
    public class Admin
    {
        [Key]
        public int AdminId{get;set;}

        [Required]
        [StringLength(100)]
        public string Username{get;set;} = string.Empty;

        [Required]
        [StringLength(255)]
        public string PasswordHash{get;set;} = string.Empty;
    }
}