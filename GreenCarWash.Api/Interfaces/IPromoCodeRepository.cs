using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GreenCarWash.Api.Models;

namespace GreenCarWash.Api.Interfaces
{
    public interface IPromoCodeRepository
    {
        Task<Promo_code?> GetByCodeAsync(string code);
        Task AddAsync(Promo_code promo);
        Task UpdateAsync(Promo_code promo);
    }
}