using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Options;

using MalakaBookFest.API.Controllers;
using MalakaBookFest.Application.Common;
using MalakaBookFest.Application.DTOs.Ticket;
using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Enums;
using MalakaBookFest.Core.Interfaces.Services;
using MalakaBookFest.Infrastructure.Configuration;
using MalakaBookFest.Application.Tables;

namespace API_TESTING.TEST_ENDPOINT_TICKET
{
    [TestClass]
    public sealed class TicketControllerTests
    {
        private Mock<ITicketService> _mockTicketService;
        private Guid _mockRequesterId;

        [TestInitialize]
        public void Setup()
        {
            _mockTicketService = new Mock<ITicketService>();
            _mockRequesterId = Guid.NewGuid();
        }

        private TicketController CreateController()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _mockRequesterId.ToString())
            }, "mock"));

            var priceTable = new TicketPriceTable(Options.Create(new TicketConfig()));

            var controller = new TicketController(_mockTicketService.Object, priceTable)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };

            return controller;
        }

        // POSITIVE TESTS

        [TestMethod]
        public async Task GetMyTickets_ShouldReturn200Ok_WhenCalledByAuthenticatedUser()
        {
            var mockTickets = new List<Ticket>
            {
                new Ticket
                {
                    TicketId    = Guid.NewGuid(),
                    UserId      = _mockRequesterId,
                    Type        = TicketType.SingleDay,
                    Status      = TicketStatus.Active,
                    QrCode      = "QR-MY001",
                    PricePaid   = 75000,
                    PurchasedAt = DateTime.UtcNow,
                    ValidDate   = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5))
                },
                new Ticket
                {
                    TicketId    = Guid.NewGuid(),
                    UserId      = _mockRequesterId,
                    Type        = TicketType.VIP,
                    Status      = TicketStatus.Used,
                    QrCode      = "QR-MY002",
                    PricePaid   = 150000,
                    PurchasedAt = DateTime.UtcNow.AddDays(-10),
                    ValidDate   = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-3))
                }
            };

            _mockTicketService.Setup(s => s.GetTicketsByUserAsync(_mockRequesterId))
                              .ReturnsAsync(mockTickets);

            var controller = CreateController();

            var actionResult = await controller.GetMyTickets();
            var okResult = actionResult.Result as OkObjectResult;
            
            Assert.IsNotNull(okResult, "Expected OkObjectResult.");
            Assert.AreEqual(200, okResult.StatusCode);
            var apiResponse = okResult.Value as ApiResponse<IEnumerable<TicketDto>>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual(2, apiResponse.Data.Count());
        }

        [TestMethod]
        public async Task PurchaseTicket_ShouldReturn200Ok_WhenDataIsValid()
        {
            var purchaseDto = new PurchaseTicketDto
            {
                Type = TicketType.SingleDay,
                ValidDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))
            };

            var createdTicket = new Ticket
            {
                TicketId = Guid.NewGuid(),
                UserId = _mockRequesterId,
                Type = purchaseDto.Type,
                Status = TicketStatus.Active,
                QrCode = "QR-GENERATED-XYZ",
                PricePaid = 75000,
                PurchasedAt = DateTime.UtcNow,
                ValidDate = purchaseDto.ValidDate
            };

            _mockTicketService.Setup(s => s.PurchaseTicketAsync(It.IsAny<Guid>(), purchaseDto.Type, purchaseDto.ValidDate))
                              .ReturnsAsync(createdTicket);

            var controller = CreateController();
            
            var actionResult = await controller.Purchase(purchaseDto);
            var okResult = actionResult.Result as OkObjectResult;

            Assert.IsNotNull(okResult, "Expected OkObjectResult.");
            Assert.AreEqual(200, okResult.StatusCode);
            var apiResponse = okResult.Value as ApiResponse<TicketDto>;
            Assert.IsNotNull(apiResponse);
            Assert.IsNotNull(apiResponse.Data);
            Assert.AreEqual(TicketStatus.Active, apiResponse.Data.Status);
            Assert.IsFalse(string.IsNullOrEmpty(apiResponse.Data.QrCode),
                           "QR code should be generated upon purchase.");
        }

        [TestMethod]
        public async Task CancelTicket_ShouldReturn200Ok_WhenOwnerCancels()
        {
            var ticketId = Guid.NewGuid();
            var mockTicket = new Ticket
            {
                TicketId = ticketId,
                UserId = _mockRequesterId,
                Type = TicketType.SingleDay,
                Status = TicketStatus.Cancelled,
                QrCode = "QR-CANCELLED",
                PricePaid = 75000,
                PurchasedAt = DateTime.UtcNow.AddDays(-1),
                ValidDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(6))
            };

            _mockTicketService.Setup(s => s.CancelTicketAsync(ticketId, _mockRequesterId))
                              .ReturnsAsync(mockTicket);

            var controller = CreateController();

            var actionResult = await controller.Cancel(ticketId);
            var okResult = actionResult.Result as OkObjectResult;

            Assert.IsNotNull(okResult, "Expected OkObjectResult.");
            Assert.AreEqual(200, okResult.StatusCode);
            var apiResponse = okResult.Value as ApiResponse<TicketDto>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual(TicketStatus.Cancelled, apiResponse.Data.Status);
        }
        // POSITIVE TESTS

        [TestMethod]
        public void GetPrices_ShouldReturn200Ok_WithPriceKeys()
        {
            var controller = CreateController();

            var actionResult = controller.GetPrices();
            var okResult = actionResult.Result as OkObjectResult;

            Assert.IsNotNull(okResult, "Expected OkObjectResult.");
            Assert.AreEqual(200, okResult.StatusCode);
            var apiResponse = okResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            var dict = apiResponse.Data as IReadOnlyDictionary<string, decimal>;
            Assert.IsNotNull(dict);
            Assert.IsTrue(dict.ContainsKey(TicketType.SingleDay.ToString()));
            Assert.IsTrue(dict.ContainsKey(TicketType.AllAccess.ToString()));
            Assert.IsTrue(dict.ContainsKey(TicketType.VIP.ToString()));
        }

        [TestMethod]
        public async Task Scan_ShouldReturn200Ok_WhenQrCodeExists()
        {
            var qr = "QR-SCAN-001";
            var mocked = new Ticket
            {
                TicketId = Guid.NewGuid(),
                UserId = _mockRequesterId,
                Type = TicketType.SingleDay,
                Status = TicketStatus.Active,
                QrCode = qr,
                PricePaid = 50000,
                PurchasedAt = DateTime.UtcNow,
                ValidDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
            };

            _mockTicketService.Setup(s => s.ScanTicketAsync(qr)).ReturnsAsync(mocked);
            var controller = CreateController();

            var actionResult = await controller.Scan(qr);
            var okResult = actionResult.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var apiResponse = okResult.Value as ApiResponse<TicketDto>;
            Assert.IsNotNull(apiResponse);
            Assert.IsNotNull(apiResponse.Data);
            Assert.AreEqual(qr, apiResponse.Data.QrCode);
        }

        // NEGATIVE TESTS

        [TestMethod]
        public async Task CancelTicket_ShouldReturn400BadRequest_WhenIdIsEmpty()
        {
            var controller = CreateController();

            var actionResult = await controller.Cancel(Guid.Empty);
            var badResult = actionResult.Result as BadRequestObjectResult;

            Assert.IsNotNull(badResult);
            Assert.AreEqual(400, badResult.StatusCode);
            var apiResponse = badResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual("Ticket id is required.", apiResponse.Message);
        }

        [TestMethod]
        public async Task Scan_ShouldReturn400BadRequest_WhenQrCodeMissing()
        {
            var controller = CreateController();

            var actionResult = await controller.Scan(string.Empty);
            var badResult = actionResult.Result as BadRequestObjectResult;

            Assert.IsNotNull(badResult);
            Assert.AreEqual(400, badResult.StatusCode);
            var apiResponse = badResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual("qrCode is required.", apiResponse.Message);
        }

        [TestMethod]
        public async Task Purchase_ShouldPropagateException_WhenServiceThrows()
        {
            var purchaseDto = new PurchaseTicketDto
            {
                Type = TicketType.SingleDay,
                ValidDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))
            };

            _mockTicketService.Setup(s => s.PurchaseTicketAsync(It.IsAny<Guid>(), purchaseDto.Type, purchaseDto.ValidDate))
                              .ThrowsAsync(new InvalidOperationException("sold out"));

            var controller = CreateController();
            try
            {
                await controller.Purchase(purchaseDto);
                Assert.Fail("Expected InvalidOperationException was not thrown.");
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("sold out", ex.Message);
            }
        }
    }
}
