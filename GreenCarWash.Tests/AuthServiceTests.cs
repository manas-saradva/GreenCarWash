using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GreenCarWash.Api.DTOs.RequestDtos;
using GreenCarWash.Api.DTOs.ResponseDtos;
using GreenCarWash.Api.Helpers;
using GreenCarWash.Api.Interfaces;
using GreenCarWash.Api.Models;
using GreenCarWash.Api.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace GreenCarWash.Tests
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<IUserRepository> _userRepoMock;
        private Mock<IWasherRepository> _washerRepoMock;
        private Mock<IAdminRepository> _adminRepoMock;
        private JwtHelper _jwtHelper;
        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _washerRepoMock = new Mock<IWasherRepository>();
            _adminRepoMock = new Mock<IAdminRepository>();
            
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["Jwt:Key"]).Returns("A_Very_Long_Secret_Key_For_Green_Car_Wash_Api_2026_Secure_Hash");
            configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            configMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
            
            _jwtHelper = new JwtHelper(configMock.Object);
            var passwordHasher = new PasswordHasher();
            _authService = new AuthService(_userRepoMock.Object, _washerRepoMock.Object, _adminRepoMock.Object, _jwtHelper, passwordHasher);
        }

        [Test]
        public async Task RegisterAsync_ValidCustomer_Succeeds()
        {
            // Arrange
            var req = new RegisterRequestDto { Name = "John", Email = "john@test.com", Password = "pass", Role = "Customer" };
            _userRepoMock.Setup(x => x.FindByEmailAsync(req.Email)).ReturnsAsync((Customer?)null);
            _userRepoMock.Setup(x => x.AddAsync(It.IsAny<Customer>())).Callback<Customer>(c => c.CustomerId = 1).ReturnsAsync((Customer c) => c);

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await _authService.RegisterAsync(req));
            _userRepoMock.Verify(x => x.AddAsync(It.IsAny<Customer>()), Times.Once);
        }

        [Test]
        public void RegisterAsync_ExistingEmail_ThrowsInvalidOperationException()
        {
            var req = new RegisterRequestDto { Name = "John", Email = "john@test.com", Password = "pass", Role = "Customer" };
            _userRepoMock.Setup(x => x.FindByEmailAsync(req.Email)).ReturnsAsync(new Customer());

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _authService.RegisterAsync(req));
        }

        [Test]
        public async Task RegisterAsync_WhenRoleWasher_Succeeds()
        {
            var req = new RegisterRequestDto { Name = "Jane", Email = "jane@test.com", Password = "pass", Role = "Washer" };
            _washerRepoMock.Setup(x => x.FindByEmailAsync(req.Email)).ReturnsAsync((Washer?)null);
            _washerRepoMock.Setup(x => x.AddAsync(It.IsAny<Washer>())).Callback<Washer>(w => w.WasherId = 1).ReturnsAsync((Washer w) => w);

            Assert.DoesNotThrowAsync(async () => await _authService.RegisterAsync(req));
            _washerRepoMock.Verify(x => x.AddAsync(It.IsAny<Washer>()), Times.Once);
        }

        [Test]
        public void RegisterAsync_InvalidRole_ThrowsArgumentException()
        {
            var req = new RegisterRequestDto { Name = "John", Email = "john@test.com", Password = "pass", Role = "Manager" };

            Assert.ThrowsAsync<ArgumentException>(async () => await _authService.RegisterAsync(req));
        }

        [Test]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            var req = new LoginRequestDto { Email = "john@test.com", Password = "pass" };
            var user = new Customer { CustomerId = 1, Email = "john@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"), IsActive = true };
            
            _userRepoMock.Setup(x => x.FindByEmailAsync(req.Email)).ReturnsAsync(user);

            var res = await _authService.LoginAsync(req);

            Assert.IsNotNull(res);
            Assert.IsNotNull(res.Token);
            Assert.AreEqual("Customer", res.Role);
        }

        [Test]
        public void LoginAsync_UserNotFound_ThrowsException()
        {
            var req = new LoginRequestDto { Email = "notfound@test.com", Password = "pass" };
            _userRepoMock.Setup(x => x.FindByEmailAsync(req.Email)).ReturnsAsync((Customer?)null);
            _washerRepoMock.Setup(x => x.FindByEmailAsync(req.Email)).ReturnsAsync((Washer?)null);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _authService.LoginAsync(req));
        }

        [Test]
        public void LoginAsync_InvalidPassword_ThrowsException()
        {
            var req = new LoginRequestDto { Email = "john@test.com", Password = "wrongpass" };
            var user = new Customer { CustomerId = 1, Email = "john@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("rightpass"), IsActive = true };
            
            _userRepoMock.Setup(x => x.FindByEmailAsync(req.Email)).ReturnsAsync(user);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _authService.LoginAsync(req));
        }

        [Test]
        public async Task LoginAsync_WasherLogin_ReturnsToken()
        {
            var req = new LoginRequestDto { Email = "washer@test.com", Password = "pass" };
            var washer = new Washer { WasherId = 1, Email = "washer@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"), IsActive = true };

            _userRepoMock.Setup(x => x.FindByEmailAsync(req.Email)).ReturnsAsync((Customer?)null);
            _washerRepoMock.Setup(x => x.FindByEmailAsync(req.Email)).ReturnsAsync(washer);

            var res = await _authService.LoginAsync(req);

            Assert.IsNotNull(res);
            Assert.IsNotNull(res.Token);
            Assert.AreEqual("Washer", res.Role);
            Assert.AreEqual(1, res.UserId);
        }

        [Test]
        public void LoginAsync_InactiveCustomer_ThrowsException()
        {
            var req = new LoginRequestDto { Email = "john@test.com", Password = "pass" };
            var user = new Customer { CustomerId = 1, Email = "john@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"), IsActive = false };
            
            _userRepoMock.Setup(x => x.FindByEmailAsync(req.Email)).ReturnsAsync(user);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _authService.LoginAsync(req));
        }

        [Test]
        public async Task AdminLoginAsync_ValidCredentials_ReturnsToken()
        {
            var req = new AdminLoginRequestDto { Username = "admin", Password = "adminpass" };
            var admin = new Admin { AdminId = 1, Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("adminpass") };

            _adminRepoMock.Setup(x => x.GetByUsernameAsync(req.Username)).ReturnsAsync(admin);

            var res = await _authService.AdminLoginAsync(req);

            Assert.IsNotNull(res);
            Assert.IsNotNull(res.Token);
            Assert.AreEqual("Admin", res.Role);
            Assert.AreEqual(1, res.UserId);
        }

        [Test]
        public void AdminLoginAsync_InvalidCredentials_ThrowsException()
        {
            var req = new AdminLoginRequestDto { Username = "admin", Password = "wrongpass" };

            _adminRepoMock.Setup(x => x.GetByUsernameAsync(req.Username)).ReturnsAsync((Admin?)null);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _authService.AdminLoginAsync(req));
        }
    }
}
