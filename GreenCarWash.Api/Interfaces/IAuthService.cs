using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenCarWash.Api.DTOs.RequestDtos;
using GreenCarWash.Api.DTOs.ResponseDtos;

namespace GreenCarWash.Api.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequestDto dto);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
        Task<AuthResponseDto> AdminLoginAsync(AdminLoginRequestDto dto);
    }
}