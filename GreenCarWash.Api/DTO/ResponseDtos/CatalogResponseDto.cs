using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.DTOs.ResponseDtos
{
    public class CatalogResponseDto
    {
        public List<CatalogPlanDto> Plans{get;set;} = new List<CatalogPlanDto>();
        public List<CatalogAddOnDto> AddOns{get;set;} = new List<CatalogAddOnDto>();
    }
}