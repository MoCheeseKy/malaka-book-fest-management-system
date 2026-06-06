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
using MalakaBookFest.Application.DTOs.Book;
using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Interfaces.Services;

namespace API_TESTING.TEST_ENDPOINT_BOOK
{
    [TestClass]
    public sealed class BookControllerTests
    {
        private Mock<IBookService> _mockBookService;
        private Guid _mockRequesterId;

        [TestInitialize]
        public void Setup()
        {
            _mockBookService = new Mock<IBookService>();
            _mockRequesterId = Guid.NewGuid();
        }

        private BookController CreateController()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _mockRequesterId.ToString())
            }, "mock"));

            var controller = new BookController(_mockBookService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };

            return controller;
        }

        [TestMethod]
        public async Task GetByBooth_ShouldReturn200Ok_WhenCalled()
        {
            var boothId = Guid.NewGuid();
            var mockBooks = new List<Book>
            {
                new Book { BookId = Guid.NewGuid(), BoothId = boothId, Title = "Clean Code" }
            };

            _mockBookService.Setup(s => s.GetBooksByBoothAsync(boothId))
                            .ReturnsAsync(mockBooks);

            var controller = CreateController();
            var actionResult = await controller.GetByBooth(boothId);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult.");
            Assert.AreEqual(200, okResult.StatusCode);
            var apiResponse = okResult.Value as ApiResponse<IEnumerable<BookDto>>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual(1, apiResponse.Data.Count());
        }

        [TestMethod]
        public async Task AddBook_ShouldReturn200Ok_WhenDataIsValid()
        {
            var boothId = Guid.NewGuid();
            var createDto = new CreateBookDto { Title = "Refactoring", Author = "Martin Fowler", Price = 150000 };
            var createdBook = new Book
            {
                BookId = Guid.NewGuid(),
                BoothId = boothId,
                Title = createDto.Title,
                Author = createDto.Author
            };

            _mockBookService.Setup(s => s.AddBookAsync(It.IsAny<Book>(), _mockRequesterId))
                            .ReturnsAsync(createdBook);

            var controller = CreateController();

            var actionResult = await controller.AddBook(boothId, createDto);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateBook_ShouldReturn200Ok_WhenBookExists()
        {
            var boothId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var updateDto = new UpdateBookDto { Title = "Updated Title" };
            var existingBook = new Book { BookId = bookId, BoothId = boothId, Title = "Old Title" };
            var updatedBook = new Book { BookId = bookId, BoothId = boothId, Title = "Updated Title" };

            _mockBookService.Setup(s => s.GetBookByIdAsync(bookId))
                            .ReturnsAsync(existingBook);
            _mockBookService.Setup(s => s.UpdateBookAsync(bookId, existingBook, _mockRequesterId))
                            .ReturnsAsync(updatedBook);

            var controller = CreateController();
            var actionResult = await controller.UpdateBook(boothId, bookId, updateDto);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task DeleteBook_ShouldReturn200Ok_WhenDeleteIsSuccessful()
        {
            var boothId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            _mockBookService.Setup(s => s.DeleteBookAsync(bookId, _mockRequesterId))
                            .Returns(Task.CompletedTask);

            var controller = CreateController();
            var actionResult = await controller.DeleteBook(boothId, bookId);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }
    }
}