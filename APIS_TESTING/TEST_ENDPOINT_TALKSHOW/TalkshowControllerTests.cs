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
            var mockTalkshows = new List<Talkshow>
            {
                new Talkshow
                {
                    TalkshowId = Guid.NewGuid(),
                    Title = "Tech Talk 1",
                    SpeakerName = "Speaker 1",
                    SpeakerBio = null,
                    Venue = "Hall A",
                    StartTime = DateTime.UtcNow,
                    EndTime = DateTime.UtcNow.AddHours(1),
                    MaxCapacity = 100,
                    Registrations = new List<TalkshowRegistration>(),
                    Status = TalkshowStatus.Scheduled
                },
                new Talkshow
                {
                    TalkshowId = Guid.NewGuid(),
                    Title = "Tech Talk 2",
                    SpeakerName = "Speaker 2",
                    SpeakerBio = null,
                    Venue = "Hall B",
                    StartTime = DateTime.UtcNow,
                    EndTime = DateTime.UtcNow.AddHours(1),
                    MaxCapacity = 50,
                    Registrations = new List<TalkshowRegistration>(),
                    Status = TalkshowStatus.Scheduled
                }
            };

            _mockTalkshowService.Setup(s => s.GetAllTalkshowsAsync())
                                .ReturnsAsync(mockTalkshows);

            var controller = CreateController();
            var actionResult = await controller.GetAll();

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
            var talkshowId = Guid.NewGuid();
            var mockTalkshow = new Talkshow
            {
                TalkshowId = talkshowId,
                Title = "Design Patterns",
                SpeakerName = "Jane Doe",
                SpeakerBio = "About design patterns",
                Venue = "Room 101",
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                MaxCapacity = 100,
                Registrations = new List<TalkshowRegistration>(),
                Status = TalkshowStatus.Scheduled
            };

            _mockTalkshowService.Setup(s => s.GetTalkshowByIdAsync(talkshowId))
                                .ReturnsAsync(mockTalkshow);

            var controller = CreateController();
            var actionResult = await controller.GetById(talkshowId);

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
                SpeakerBio = null,
                Venue = createDto.Venue,
                StartTime = createDto.StartTime,
                EndTime = createDto.EndTime,
                MaxCapacity = createDto.MaxCapacity,
                Registrations = new List<TalkshowRegistration>(),
                Status = TalkshowStatus.Scheduled
            };

            _mockTalkshowService.Setup(s => s.CreateTalkshowAsync(It.IsAny<Talkshow>()))
                                .ReturnsAsync(createdTalkshow);

            var controller = CreateController();
            var actionResult = await controller.Create(createDto);

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
            var talkshowId = Guid.NewGuid();
            var updateDto = new UpdateTalkshowDto { Title = "Updated Talkshow Title" };

            var existingTalkshow = new Talkshow
            {
                TalkshowId = talkshowId,
                Title = "Old Title",
                SpeakerName = "Old Speaker",
                SpeakerBio = null,
                Venue = "Old Venue",
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                MaxCapacity = 100,
                Registrations = new List<TalkshowRegistration>(),
                Status = TalkshowStatus.Scheduled
            };

            var updatedTalkshow = new Talkshow
            {
                TalkshowId = talkshowId,
                Title = "Updated Talkshow Title",
                SpeakerName = "Old Speaker",
                SpeakerBio = null,
                Venue = "Old Venue",
                StartTime = existingTalkshow.StartTime,
                EndTime = existingTalkshow.EndTime,
                MaxCapacity = existingTalkshow.MaxCapacity,
                Registrations = existingTalkshow.Registrations,
                Status = TalkshowStatus.Scheduled
            };

            _mockTalkshowService.Setup(s => s.GetTalkshowByIdAsync(talkshowId))
                                .ReturnsAsync(existingTalkshow);
            _mockTalkshowService.Setup(s => s.UpdateTalkshowAsync(talkshowId, existingTalkshow))
                                .ReturnsAsync(updatedTalkshow);

            var controller = CreateController();
            var actionResult = await controller.Update(talkshowId, updateDto);

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
            var actionResult = await controller.Register(talkshowId);

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
            var talkshowId = Guid.NewGuid();

            _mockTalkshowService.Setup(s => s.AdvanceTalkshowStatusAsync(talkshowId))
                                .Returns(Task.CompletedTask);

            var controller = CreateController();
            var actionResult = await controller.AdvanceStatus(talkshowId);

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
        public async Task GetById_ShouldReturn400BadRequest_WhenIdIsEmpty()
        {
            var controller = CreateController();

            var actionResult = await controller.GetById(Guid.Empty);

            var badResult = actionResult.Result as BadRequestObjectResult;
            Assert.IsNotNull(badResult);
            Assert.AreEqual(400, badResult.StatusCode);

            var apiResponse = badResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual("Talkshow id is required.", apiResponse.Message);
        }

        [TestMethod]
        public async Task Update_ShouldReturn400BadRequest_WhenIdIsEmpty()
        {
            var updateDto = new UpdateTalkshowDto { Title = "New Title" };
            var controller = CreateController();

            var actionResult = await controller.Update(Guid.Empty, updateDto);

            var badResult = actionResult.Result as BadRequestObjectResult;
            Assert.IsNotNull(badResult);
            Assert.AreEqual(400, badResult.StatusCode);

            var apiResponse = badResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual("Talkshow id is required.", apiResponse.Message);
        }

        [TestMethod]
        public async Task Register_ShouldReturn400BadRequest_WhenIdIsEmpty()
        {
            var controller = CreateController();

            var actionResult = await controller.Register(Guid.Empty);

            var badResult = actionResult.Result as BadRequestObjectResult;
            Assert.IsNotNull(badResult);
            Assert.AreEqual(400, badResult.StatusCode);

            var apiResponse = badResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual("Talkshow id is required.", apiResponse.Message);
        }

        [TestMethod]
        public async Task AdvanceStatus_ShouldReturn400BadRequest_WhenIdIsEmpty()
        {
            var controller = CreateController();

            var actionResult = await controller.AdvanceStatus(Guid.Empty);

            var badResult = actionResult.Result as BadRequestObjectResult;
            Assert.IsNotNull(badResult);
            Assert.AreEqual(400, badResult.StatusCode);

            var apiResponse = badResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual("Talkshow id is required.", apiResponse.Message);
        }

        #endregion
    }
}