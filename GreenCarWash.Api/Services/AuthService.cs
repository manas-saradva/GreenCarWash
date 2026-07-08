using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenCarWash.Api.DTOs.RequestDtos;
using GreenCarWash.Api.DTOs.ResponseDtos;
using GreenCarWash.Api.Helpers;
using GreenCarWash.Api.Interfaces;
using GreenCarWash.Api.Models;

namespace GreenCarWash.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IWasherRepository _washerRepo;
        private readonly IAdminRepository _adminRepo;
        private readonly JwtHelper _jwtHelper;
        private readonly PasswordHasher _passwordHasher;

        public AuthService(IUserRepository userRepo,IWasherRepository washerRepo,IAdminRepository adminRepo,
                            JwtHelper jwtHelper,PasswordHasher passwordHasher)
        {
            _userRepo = userRepo;
            _washerRepo = washerRepo;
            _adminRepo = adminRepo;
            _jwtHelper = jwtHelper;
            _passwordHasher = passwordHasher;
        }

        public async Task RegisterAsync(RegisterRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Role))
            {
                throw new ArgumentException("Role is required");
            }

            var role = request.Role.Trim();
            var hash = _passwordHasher.HashPassword(request.Password);

            if (role.Equals("Washer", StringComparison.OrdinalIgnoreCase))
            {
                if(await _washerRepo.FindByEmailAsync(request.Email) != null)
                {
                    throw new InvalidOperationException("Email already exists");
                }

                var washer = new Washer
                {
                    Name = request.Name,
                    Email = request.Email,
                    Phone = request.Phone,
                    PasswordHash = hash
                };
                await _washerRepo.AddAsync(washer);
                return;
            }
            else if(role.Equals("Customer", StringComparison.OrdinalIgnoreCase))
            {
                if(await _userRepo.FindByEmailAsync(request.Email) != null)
                {
                    throw new InvalidOperationException("Email already exists");
                }

                var customer = new Customer
                {
                    Name = request.Name,
                    Email = request.Email,
                    Phone = request.Phone,
                    PasswordHash = hash
                };
                await _userRepo.AddAsync(customer);
                return;
            }
            else
            {
                throw new ArgumentException($"Invalid role specified: {role}. Allowed roles are 'Customer' or 'Washer'");
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var customer = await _userRepo.FindByEmailAsync(request.Email);
            if(customer != null && _passwordHasher.VerifyPassword(request.Password, customer.PasswordHash))
            {
                if (!customer.IsActive)
                {
                    throw new UnauthorizedAccessException("Account is inactive");
                }

                var token = _jwtHelper.GenerateToken(customer.CustomerId,customer.Email,"Customer");

                return new AuthResponseDto
                {
                    Token = token.Token,
                    ExpiresAt = token.ExpiresAt,
                    Role = "Customer",
                    UserId = customer.CustomerId
                };
            }

            var washer = await _washerRepo.FindByEmailAsync(request.Email);

            if (washer != null && _passwordHasher.VerifyPassword(request.Password, washer.PasswordHash))
            {
                if (!washer.IsActive)
                    throw new UnauthorizedAccessException("Account is inactive");

                var token = _jwtHelper.GenerateToken(washer.WasherId, washer.Email, "Washer");

                return new AuthResponseDto
                {
                    Token = token.Token,
                    ExpiresAt = token.ExpiresAt,
                    Role = "Washer",
                    UserId = washer.WasherId
                };
            }

            throw new UnauthorizedAccessException("Invalid email or password");
        }

        public async Task<AuthResponseDto> AdminLoginAsync(AdminLoginRequestDto request){
            var admin = await _adminRepo.GetByUsernameAsync(request.Username);

            if(admin != null && _passwordHasher.VerifyPassword(request.Password, admin.PasswordHash))
            {
                var token = _jwtHelper.GenerateToken(admin.AdminId, admin.Username, "Admin");

                return new AuthResponseDto
                {
                    Token = token.Token,
                    ExpiresAt = token.ExpiresAt,
                    Role = "Admin",
                    UserId = admin.AdminId
                };
            }

            throw new UnauthorizedAccessException("Invalid admin credentials");
        }
    }
}