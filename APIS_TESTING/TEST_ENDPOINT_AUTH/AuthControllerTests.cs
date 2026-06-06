using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using MalakaBookFest.API.Controllers;
using MalakaBookFest.Application.Common;
using MalakaBookFest.Application.DTOs.Auth;
using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Enums;
using MalakaBookFest.Core.Interfaces.Services;

namespace API_TESTING.TEST_ENDPOINT_AUTH
{
    [TestClass]
    public sealed class AuthControllerTests
    {
        private Mock<IAuthService> _mockAuthService = null!;
        private User _mockUser = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockAuthService = new Mock<IAuthService>();

            _mockUser = new User
            {
                UserId = Guid.NewGuid(),
                Email = "naufal@example.com",
                FullName = "Naufal",
                Role = UserRole.Attendee,
                IsActive = true,
                PasswordHash = "hashed-password"
            };
        }

        private AuthController CreateController() => new(_mockAuthService.Object);

        private static IList<ValidationResult> ValidateDto<T>(T dto)
        {
            var context = new ValidationContext(dto!);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(dto!, context, results, validateAllProperties: true);

            return results;
        }

        private static async Task AssertThrowsAsync<TException>(Func<Task> action)
            where TException : Exception
        {
            try
            {
                await action();
                Assert.Fail($"Expected exception of type {typeof(TException).Name}.");
            }
            catch (TException)
            {
            }
        }

        [TestMethod]
        public async Task Register_ShouldReturn200Ok_WhenDataIsValid()
        {
            var dto = new RegisterRequestDto
            {
                Email = "newuser@example.com",
                Password = "Password123!",
                FullName = "New User"
            };

            _mockAuthService.Setup(service => service.RegisterAsync(dto.Email, dto.Password, dto.FullName))
                .ReturnsAsync(_mockUser);
            _mockAuthService.Setup(service => service.LoginAsync(dto.Email, dto.Password))
                .ReturnsAsync(("fake-jwt-token", _mockUser));

            var controller = CreateController();
            var actionResult = await controller.Register(dto);

            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult.");
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<LoginResponseDto>;
            Assert.IsNotNull(apiResponse, "Expected ApiResponse<LoginResponseDto>.");
            Assert.IsTrue(apiResponse.Success);
            Assert.IsNotNull(apiResponse.Data);
            Assert.AreEqual("fake-jwt-token", apiResponse.Data.Token);
            Assert.AreEqual(_mockUser.UserId, apiResponse.Data.UserId);
            Assert.AreEqual("Registration successful.", apiResponse.Message);
        }

        [TestMethod]
        public async Task Login_ShouldReturn200Ok_WhenCredentialsAreValid()
        {
            var dto = new LoginRequestDto
            {
                Email = _mockUser.Email,
                Password = "Password123!"
            };

            _mockAuthService.Setup(service => service.LoginAsync(dto.Email, dto.Password))
                .ReturnsAsync(("valid-jwt-token", _mockUser));

            var controller = CreateController();
            var actionResult = await controller.Login(dto);

            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult.");
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<LoginResponseDto>;
            Assert.IsNotNull(apiResponse, "Expected ApiResponse<LoginResponseDto>.");
            Assert.IsTrue(apiResponse.Success);
            Assert.IsNotNull(apiResponse.Data);
            Assert.AreEqual("valid-jwt-token", apiResponse.Data.Token);
            Assert.AreEqual(_mockUser.Email, apiResponse.Data.Email);
            Assert.AreEqual("Login successful.", apiResponse.Message);
        }

        [TestMethod]
        public async Task Register_ShouldThrowInvalidOperationException_WhenEmailAlreadyRegistered()
        {
            var dto = new RegisterRequestDto
            {
                Email = "existing@example.com",
                Password = "Password123!",
                FullName = "Existing User"
            };

            _mockAuthService.Setup(service => service.RegisterAsync(dto.Email, dto.Password, dto.FullName))
                .ThrowsAsync(new InvalidOperationException("Email is already registered."));

            var controller = CreateController();

            await AssertThrowsAsync<InvalidOperationException>(() => controller.Register(dto));
        }

        [TestMethod]
        public async Task Login_ShouldThrowUnauthorizedAccessException_WhenCredentialsAreInvalid()
        {
            var dto = new LoginRequestDto
            {
                Email = "wrong@example.com",
                Password = "wrong-password"
            };

            _mockAuthService.Setup(service => service.LoginAsync(dto.Email, dto.Password))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid email or password."));

            var controller = CreateController();

            await AssertThrowsAsync<UnauthorizedAccessException>(() => controller.Login(dto));
        }

        [TestMethod]
        public void RegisterRequestDto_ShouldFailValidation_WhenEmailIsEmpty()
        {
            var dto = new RegisterRequestDto
            {
                Email = string.Empty,
                Password = "Password123!",
                FullName = "New User"
            };

            var results = ValidateDto(dto);

            Assert.IsTrue(results.Any(result => result.MemberNames.Contains(nameof(RegisterRequestDto.Email))));
        }

        [TestMethod]
        public void RegisterRequestDto_ShouldFailValidation_WhenPasswordIsTooShort()
        {
            var dto = new RegisterRequestDto
            {
                Email = "newuser@example.com",
                Password = "123",
                FullName = "New User"
            };

            var results = ValidateDto(dto);

            Assert.IsTrue(results.Any(result => result.MemberNames.Contains(nameof(RegisterRequestDto.Password))));
        }

        [TestMethod]
        public void LoginRequestDto_ShouldFailValidation_WhenEmailIsInvalid()
        {
            var dto = new LoginRequestDto
            {
                Email = "not-an-email",
                Password = "Password123!"
            };

            var results = ValidateDto(dto);

            Assert.IsTrue(results.Any(result => result.MemberNames.Contains(nameof(LoginRequestDto.Email))));
        }

        [TestMethod]
        public void LoginRequestDto_ShouldFailValidation_WhenPasswordIsEmpty()
        {
            var dto = new LoginRequestDto
            {
                Email = "user@example.com",
                Password = string.Empty
            };

            var results = ValidateDto(dto);

            Assert.IsTrue(results.Any(result => result.MemberNames.Contains(nameof(LoginRequestDto.Password))));
        }
    }
}
