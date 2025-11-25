using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using APIPropertyRegistry.Controllers;
using APIPropertyRegistry.Services.Interfaces;
using APIPropertyRegistry.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace APIPropertyRegistry.Tests.Controllers
{
    public class PropertyTransactionControllerTests
    {
        private readonly Mock<IPropertyTransactionService> _mockService;
        private readonly PropertyTransactionController _controller;

        public PropertyTransactionControllerTests()
        {
            _mockService = new Mock<IPropertyTransactionService>();
            _controller = new PropertyTransactionController(_mockService.Object);
        }

        [Fact]
        public async Task Create_WithValidDto_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var createDto = TestFixtures.CreatePropertyTransactionCreateDto(1, 1, 2, 3);
            var responseDto = TestFixtures.CreatePropertyTransactionResponseDto(1, 1, 1, 2);
            _mockService.Setup(s => s.CreateTransactionAsync(createDto)).ReturnsAsync(responseDto);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(PropertyTransactionController.GetById), createdResult.ActionName);
            Assert.Equal(1, ((PropertyTransactionResponseDto)createdResult.Value!).TransactionId);
            _mockService.Verify(s => s.CreateTransactionAsync(createDto), Times.Once);
        }

        [Fact]
        public async Task Create_WithInvalidModelState_ShouldReturnBadRequest()
        {
            // Arrange
            var createDto = TestFixtures.CreatePropertyTransactionCreateDto();
            _controller.ModelState.AddModelError("PropertyId", "Required");

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult.Value);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithList()
        {
            // Arrange
            var transactions = new List<PropertyTransactionResponseDto>
            {
                TestFixtures.CreatePropertyTransactionResponseDto(1, 1, 1, 2),
                TestFixtures.CreatePropertyTransactionResponseDto(2, 2, 1, 3)
            };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(transactions);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<PropertyTransactionResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
            _mockService.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnOk()
        {
            // Arrange
            var transaction = TestFixtures.CreatePropertyTransactionResponseDto(1, 1, 1, 2);
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(transaction);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTxn = Assert.IsType<PropertyTransactionResponseDto>(okResult.Value);
            Assert.Equal(1, returnedTxn.TransactionId);
            _mockService.Verify(s => s.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((PropertyTransactionResponseDto?)null);

            // Act
            var result = await _controller.GetById(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
            _mockService.Verify(s => s.GetByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task GetByBuyer_WithValidBuyerId_ShouldReturnOk()
        {
            // Arrange
            var buyerId = 2;
            var transactions = new List<PropertyTransactionResponseDto>
            {
                TestFixtures.CreatePropertyTransactionResponseDto(1, 1, 1, buyerId),
                TestFixtures.CreatePropertyTransactionResponseDto(2, 2, 3, buyerId)
            };
            _mockService.Setup(s => s.GetByBuyerAsync(buyerId)).ReturnsAsync(transactions);

            // Act
            var result = await _controller.GetByBuyer(buyerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<PropertyTransactionResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
            _mockService.Verify(s => s.GetByBuyerAsync(buyerId), Times.Once);
        }

        [Fact]
        public async Task GetBySeller_WithValidSellerId_ShouldReturnOk()
        {
            // Arrange
            var sellerId = 1;
            var transactions = new List<PropertyTransactionResponseDto>
            {
                TestFixtures.CreatePropertyTransactionResponseDto(1, 1, sellerId, 2),
                TestFixtures.CreatePropertyTransactionResponseDto(2, 2, sellerId, 3)
            };
            _mockService.Setup(s => s.GetBySellerAsync(sellerId)).ReturnsAsync(transactions);

            // Act
            var result = await _controller.GetBySeller(sellerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<PropertyTransactionResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
            _mockService.Verify(s => s.GetBySellerAsync(sellerId), Times.Once);
        }

        [Fact]
        public async Task Verify_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var verifyDto = TestFixtures.CreatePropertyTransactionVerifyDto(1, 1, true);
            _mockService.Setup(s => s.VerifyTransactionAsync(verifyDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Verify(verifyDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockService.Verify(s => s.VerifyTransactionAsync(verifyDto), Times.Once);
        }

        [Fact]
        public async Task Verify_WithInvalidModelState_ShouldReturnBadRequest()
        {
            // Arrange
            var verifyDto = TestFixtures.CreatePropertyTransactionVerifyDto();
            _controller.ModelState.AddModelError("TransactionId", "Required");

            // Act
            var result = await _controller.Verify(verifyDto);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult.Value);
        }

        [Fact]
        public async Task Verify_WithInvalidTransactionId_ShouldReturnNotFound()
        {
            // Arrange
            var verifyDto = TestFixtures.CreatePropertyTransactionVerifyDto(999, 1, true);
            _mockService.Setup(s => s.VerifyTransactionAsync(verifyDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.Verify(verifyDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
            _mockService.Verify(s => s.VerifyTransactionAsync(verifyDto), Times.Once);
        }

        [Fact]
        public async Task Verify_WhenRejecting_ShouldReturnOkWithRejectionMessage()
        {
            // Arrange
            var verifyDto = TestFixtures.CreatePropertyTransactionVerifyDto(1, 1, false);
            _mockService.Setup(s => s.VerifyTransactionAsync(verifyDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Verify(verifyDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value!.GetType();
            var messageProperty = response.GetProperty("message");
            Assert.NotNull(messageProperty);
            _mockService.Verify(s => s.VerifyTransactionAsync(verifyDto), Times.Once);
        }
    }
}