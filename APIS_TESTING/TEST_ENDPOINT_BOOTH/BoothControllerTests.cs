using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel.DataAnnotations;

using MalakaBookFest.API.Controllers;
using MalakaBookFest.Application.Common;
using MalakaBookFest.Application.DTOs.Booth;
using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Interfaces.Services;

namespace API_TESTING.TEST_ENDPOINT_BOOTH
{
    [TestClass]
    public sealed class BoothControllerTests
    {
        private Mock<IBoothService> _mockBoothService = null!;
        private Guid _mockRequesterId;

        [TestInitialize]
        public void Setup()
        {
            _mockBoothService = new Mock<IBoothService>();
            _mockRequesterId = Guid.NewGuid();
        }

        private BoothController CreateController()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _mockRequesterId.ToString())
            }, "mock"));

            var controller = new BoothController(_mockBoothService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };

            return controller;
        }

        [TestMethod]
        public async Task GetAll_Positive_ReturnsOkAndData()
        {
            var booths = new List<Booth> { new Booth { BoothId = Guid.NewGuid(), BoothName = "B1", BoothNumber = "1" } };
            _mockBoothService.Setup(s => s.GetAllBoothsAsync()).ReturnsAsync(booths);

            var controller = CreateController();
            var result = await controller.GetAll();
            var ok = result.Result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            var api = ok.Value as ApiResponse<IEnumerable<BoothDto>>;
            Assert.IsNotNull(api);
            Assert.AreEqual(1, api.Data.Count());
        }

        [TestMethod]
        public async Task GetById_Positive_ReturnsOk_WhenFound()
        {
            var id = Guid.NewGuid();
            var booth = new Booth { BoothId = id, BoothName = "Found", BoothNumber = "F1" };
            _mockBoothService.Setup(s => s.GetBoothByIdAsync(id)).ReturnsAsync(booth);

            var controller = CreateController();
            var result = await controller.GetById(id);
            var ok = result.Result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            var api = ok.Value as ApiResponse<BoothDto>;
            Assert.IsNotNull(api);
            Assert.AreEqual(id, api.Data.BoothId);
        }


        [TestMethod]
        public async Task Create_Positive_ReturnsCreated_WhenValid()
        {
            var dto = new CreateBoothDto { BoothName = "New", BoothNumber = "N1", Description = "d", Category = MalakaBookFest.Core.Enums.BoothCategory.Other };
            var created = new Booth { BoothId = Guid.NewGuid(), OrganizerId = _mockRequesterId, BoothName = dto.BoothName, BoothNumber = dto.BoothNumber };
            _mockBoothService.Setup(s => s.CreateBoothAsync(It.IsAny<Booth>())).ReturnsAsync(created);

            var controller = CreateController();
            var result = await controller.Create(dto);
            var createdAt = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAt);
            Assert.AreEqual(201, createdAt.StatusCode);
            var api = createdAt.Value as ApiResponse<BoothDto>;
            Assert.IsNotNull(api);
            Assert.AreEqual(created.BoothId, api.Data.BoothId);
        }


        [TestMethod]
        public async Task Update_Positive_ReturnsOk_WhenAuthorized()
        {
            var id = Guid.NewGuid();
            var existing = new Booth { BoothId = id, OrganizerId = _mockRequesterId, BoothName = "Old", BoothNumber = "O1" };
            var updateDto = new UpdateBoothDto { BoothName = "NewName" };
            var updated = new Booth { BoothId = id, OrganizerId = _mockRequesterId, BoothName = "NewName", BoothNumber = "O1" };

            _mockBoothService.Setup(s => s.GetBoothByIdAsync(id)).ReturnsAsync(existing);
            _mockBoothService.Setup(s => s.UpdateBoothAsync(id, It.IsAny<Booth>(), _mockRequesterId)).ReturnsAsync(updated);

            var controller = CreateController();
            var result = await controller.Update(id, updateDto);
            var ok = result.Result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            var api = ok.Value as ApiResponse<BoothDto>;
            Assert.IsNotNull(api);
            Assert.AreEqual("NewName", api.Data.BoothName);
        }

        [TestMethod]
        public async Task GetById_Negative_ReturnsBadRequest_WhenIdEmpty()
        {
            var controller = CreateController();
            var result = await controller.GetById(Guid.Empty);
            var bad = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(bad);
            Assert.AreEqual(400, bad.StatusCode);
            var api = bad.Value as ApiResponse<object>;
            Assert.IsNotNull(api);
            Assert.IsFalse(api.Success);
            Assert.AreEqual("Booth id is required.", api.Message);
        }

        [TestMethod]
        public async Task Update_Negative_ReturnsBadRequest_WhenIdEmpty()
        {
            var controller = CreateController();
            var result = await controller.Update(Guid.Empty, new UpdateBoothDto());
            var bad = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(bad);
            Assert.AreEqual(400, bad.StatusCode);
            var api = bad.Value as ApiResponse<object>;
            Assert.IsNotNull(api);
            Assert.IsFalse(api.Success);
            Assert.AreEqual("Booth id is required.", api.Message);
        }

        [TestMethod]
        public void Create_Negative_ReturnsBadRequest_WhenModelInvalid()
        {
            var controller = CreateController();
            var dto = new CreateBoothDto { BoothName = "", BoothNumber = "" };

            var ctx = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var valid = Validator.TryValidateObject(dto, ctx, results, true);
            Assert.IsFalse(valid);
            Assert.IsTrue(results.Count > 0);

            var errors = results.Select(r => r.ErrorMessage).ToList();

            var bad = controller.BadRequest(ApiResponse<object>.Fail("Validation failed", errors)) as BadRequestObjectResult;
            Assert.IsNotNull(bad);
            Assert.AreEqual(400, bad.StatusCode);
            var api = bad.Value as ApiResponse<object>;
            Assert.IsNotNull(api);
            Assert.IsFalse(api.Success);
            Assert.IsNotNull(api.Errors);
            Assert.IsTrue(api.Errors.Any());
        }

        [TestMethod]
        public void Update_Negative_ReturnsBadRequest_WhenModelInvalid()
        {
            var controller = CreateController();
            var dto = new UpdateBoothDto { BoothName = "A" };

            var ctx = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var valid = Validator.TryValidateObject(dto, ctx, results, true);
            Assert.IsFalse(valid);
            Assert.IsTrue(results.Count > 0);

            var errors = results.Select(r => r.ErrorMessage).ToList();

            var bad = controller.BadRequest(ApiResponse<object>.Fail("Validation failed", errors)) as BadRequestObjectResult;
            Assert.IsNotNull(bad);
            Assert.AreEqual(400, bad.StatusCode);
            var api = bad.Value as ApiResponse<object>;
            Assert.IsNotNull(api);
            Assert.IsFalse(api.Success);
            Assert.IsNotNull(api.Errors);
            Assert.IsTrue(api.Errors.Any());
        }

        [TestMethod]
        public async Task Delete_Positive_ReturnsOk_WhenDeleted()
        {
            var id = Guid.NewGuid();
            _mockBoothService.Setup(s => s.DeleteBoothAsync(id, _mockRequesterId)).Returns(Task.CompletedTask);

            var controller = CreateController();
            var result = await controller.Delete(id);
            var ok = result.Result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
        }

        [TestMethod]
        public async Task Delete_Negative_ReturnsBadRequest_WhenIdEmpty()
        {
            var controller = CreateController();
            var result = await controller.Delete(Guid.Empty);
            var bad = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(bad);
            Assert.AreEqual(400, bad.StatusCode);
            var api = bad.Value as ApiResponse<object>;
            Assert.IsNotNull(api);
            Assert.IsFalse(api.Success);
            Assert.AreEqual("Booth id is required.", api.Message);
        }
    }
}
