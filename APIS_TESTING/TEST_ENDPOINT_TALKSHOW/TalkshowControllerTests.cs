using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

using MalakaBookFest.API.Controllers;
using MalakaBookFest.Application.Common;
using MalakaBookFest.Application.DTOs.Talkshow;
using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Enums;
using MalakaBookFest.Core.Interfaces.Services;

namespace API_TESTING.TEST_ENDPOINT_TALKSHOW
{
    [TestClass]
    public sealed class TalkshowControllerTests
    {
        private Mock<ITalkshowService> _mockTalkshowService;
        private Guid _mockRequesterId;

        [TestInitialize]
        public void Setup()
        {
            _mockTalkshowService = new Mock<ITalkshowService>();
            _mockRequesterId = Guid.NewGuid();
        }

        private TalkshowController CreateController()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _mockRequesterId.ToString())
            }, "mock"));

            var controller = new TalkshowController(_mockTalkshowService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };

            return controller;
        }

        #region Positive Scenarios

        [TestMethod]
        public async Task GetAll_ShouldReturn200Ok_WhenCalled()
        {
            // Arrange
            var mockTalkshows = new List<Talkshow>
            {
                new Talkshow { TalkshowId = Guid.NewGuid(), Title = "Tech Talk 1", MaxCapacity = 100 },
                new Talkshow { TalkshowId = Guid.NewGuid(), Title = "Tech Talk 2", MaxCapacity = 50 }
            };

            _mockTalkshowService.Setup(s => s.GetAllTalkshowsAsync())
                                .ReturnsAsync(mockTalkshows);

            var controller = CreateController();

            // Act
            var actionResult = await controller.GetAll();

            // Assert
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult.");
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<IEnumerable<TalkshowDto>>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual(2, apiResponse.Data.Count());
        }

        [TestMethod]
        public async Task GetById_ShouldReturn200Ok_WhenTalkshowExists()
        {
            // Arrange
            var talkshowId = Guid.NewGuid();
            var mockTalkshow = new Talkshow { TalkshowId = talkshowId, Title = "Design Patterns" };

            _mockTalkshowService.Setup(s => s.GetTalkshowByIdAsync(talkshowId))
                                .ReturnsAsync(mockTalkshow);

            var controller = CreateController();

            // Act
            var actionResult = await controller.GetById(talkshowId);

            // Assert
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<TalkshowDto>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Design Patterns", apiResponse.Data.Title);
        }

        [TestMethod]
        public async Task Create_ShouldReturn201Created_WhenDataIsValid()
        {
            // Arrange
            var createDto = new CreateTalkshowDto
            {
                Title = "Future of AI",
                SpeakerName = "John Doe",
                Venue = "Main Hall",
                StartTime = DateTime.UtcNow.AddDays(1),
                EndTime = DateTime.UtcNow.AddDays(1).AddHours(2),
                MaxCapacity = 200
            };

            var createdTalkshow = new Talkshow
            {
                TalkshowId = Guid.NewGuid(),
                Title = createDto.Title,
                SpeakerName = createDto.SpeakerName,
                Venue = createDto.Venue
            };

            _mockTalkshowService.Setup(s => s.CreateTalkshowAsync(It.IsAny<Talkshow>()))
                                .ReturnsAsync(createdTalkshow);

            var controller = CreateController();

            // Act
            var actionResult = await controller.Create(createDto);

            // Assert
            var createdResult = actionResult.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult, "Expected CreatedAtActionResult.");
            Assert.AreEqual(201, createdResult.StatusCode);

            var apiResponse = createdResult.Value as ApiResponse<TalkshowDto>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Future of AI", apiResponse.Data.Title);
        }

        [TestMethod]
        public async Task Update_ShouldReturn200Ok_WhenTalkshowExists()
        {
            // Arrange
            var talkshowId = Guid.NewGuid();
            var updateDto = new UpdateTalkshowDto { Title = "Updated Talkshow Title" };

            var existingTalkshow = new Talkshow { TalkshowId = talkshowId, Title = "Old Title" };
            var updatedTalkshow = new Talkshow { TalkshowId = talkshowId, Title = "Updated Talkshow Title" };

            _mockTalkshowService.Setup(s => s.GetTalkshowByIdAsync(talkshowId))
                                .ReturnsAsync(existingTalkshow);
            _mockTalkshowService.Setup(s => s.UpdateTalkshowAsync(talkshowId, existingTalkshow))
                                .ReturnsAsync(updatedTalkshow);

            var controller = CreateController();

            // Act
            var actionResult = await controller.Update(talkshowId, updateDto);

            // Assert
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<TalkshowDto>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Updated Talkshow Title", apiResponse.Data.Title);
        }

        [TestMethod]
        public async Task Register_ShouldReturn200Ok_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var talkshowId = Guid.NewGuid();
            var mockRegistration = new TalkshowRegistration
            {
                RegistrationId = Guid.NewGuid(),
                TalkshowId = talkshowId,
                UserId = _mockRequesterId,
                SeatCode = "SEAT-0001",
                RegisteredAt = DateTime.UtcNow
            };

            _mockTalkshowService.Setup(s => s.RegisterAttendeeAsync(_mockRequesterId, talkshowId))
                                .ReturnsAsync(mockRegistration);

            var controller = CreateController();

            // Act
            var actionResult = await controller.Register(talkshowId);

            // Assert
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual("Successfully registered for talkshow.", apiResponse.Message);
        }

        [TestMethod]
        public async Task AdvanceStatus_ShouldReturn200Ok_WhenCalled()
        {
            // Arrange
            var talkshowId = Guid.NewGuid();

            _mockTalkshowService.Setup(s => s.AdvanceTalkshowStatusAsync(talkshowId))
                                .Returns(Task.CompletedTask);

            var controller = CreateController();

            // Act
            var actionResult = await controller.AdvanceStatus(talkshowId);

            // Assert
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual("Talkshow status advanced successfully.", apiResponse.Message);
        }

        #endregion

        #region Negative Scenarios

        [TestMethod]
        public async Task GetById_ShouldThrowKeyNotFoundException_WhenTalkshowDoesNotExist()
        {
            // Arrange
            var talkshowId = Guid.NewGuid();

            _mockTalkshowService.Setup(s => s.GetTalkshowByIdAsync(talkshowId))
                                .ThrowsAsync(new KeyNotFoundException($"Talkshow with ID {talkshowId} not found."));

            var controller = CreateController();

            // Act & Assert
            var exception = await Assert.ThrowsExactlyAsync<KeyNotFoundException>(() => controller.GetById(talkshowId));
            Assert.AreEqual($"Talkshow with ID {talkshowId} not found.", exception.Message);
        }

        [TestMethod]
        public async Task Create_ShouldThrowArgumentException_WhenDataIsInvalid()
        {
            // Arrange
            var createDto = new CreateTalkshowDto
            {
                Title = "", // Invalid: Kosong
                SpeakerName = "John Doe"
            };

            _mockTalkshowService.Setup(s => s.CreateTalkshowAsync(It.IsAny<Talkshow>()))
                                .ThrowsAsync(new ArgumentException("Title cannot be empty."));

            var controller = CreateController();

            // Act & Assert
            var exception = await Assert.ThrowsExactlyAsync<ArgumentException>(() => controller.Create(createDto));
            Assert.AreEqual("Title cannot be empty.", exception.Message);
        }

        [TestMethod]
        public async Task Update_ShouldThrowKeyNotFoundException_WhenTalkshowDoesNotExist()
        {
            // Arrange
            var talkshowId = Guid.NewGuid();
            var updateDto = new UpdateTalkshowDto { Title = "New Title" };

            _mockTalkshowService.Setup(s => s.GetTalkshowByIdAsync(talkshowId))
                                .ThrowsAsync(new KeyNotFoundException("Talkshow not found."));

            var controller = CreateController();

            // Act & Assert
            var exception = await Assert.ThrowsExactlyAsync<KeyNotFoundException>(() => controller.Update(talkshowId, updateDto));
            Assert.AreEqual("Talkshow not found.", exception.Message);
        }

        [TestMethod]
        public async Task Register_ShouldThrowInvalidOperationException_WhenTalkshowIsFull()
        {
            // Arrange
            var talkshowId = Guid.NewGuid();

            _mockTalkshowService.Setup(s => s.RegisterAttendeeAsync(_mockRequesterId, talkshowId))
                                .ThrowsAsync(new InvalidOperationException("Talkshow capacity is already full."));

            var controller = CreateController();

            // Act & Assert
            var exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => controller.Register(talkshowId));
            Assert.AreEqual("Talkshow capacity is already full.", exception.Message);
        }

        [TestMethod]
        public async Task Register_ShouldThrowInvalidOperationException_WhenUserAlreadyRegistered()
        {
            // Arrange
            var talkshowId = Guid.NewGuid();

            _mockTalkshowService.Setup(s => s.RegisterAttendeeAsync(_mockRequesterId, talkshowId))
                                .ThrowsAsync(new InvalidOperationException("User is already registered for this talkshow."));

            var controller = CreateController();

            // Act & Assert
            var exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => controller.Register(talkshowId));
            Assert.AreEqual("User is already registered for this talkshow.", exception.Message);
        }

        [TestMethod]
        public async Task AdvanceStatus_ShouldThrowInvalidOperationException_WhenTransitionIsInvalid()
        {
            var talkshowId = Guid.NewGuid();

            _mockTalkshowService.Setup(s => s.AdvanceTalkshowStatusAsync(talkshowId))
                                .ThrowsAsync(new InvalidOperationException("Cannot advance status from Completed."));

            var controller = CreateController();

            var exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => controller.AdvanceStatus(talkshowId));
            Assert.AreEqual("Cannot advance status from Completed.", exception.Message);
        }

        #endregion
    }
}