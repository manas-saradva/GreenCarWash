using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.DTOs.ResponseDtos
{
    public class CatalogAddOnDto
    {
        public int AddOnId{get;set;}
        public string Name{get;set;} = string.Empty;
        public string Description{get;set;} =string.Empty;
        public decimal Price{get;set;}
    }
}