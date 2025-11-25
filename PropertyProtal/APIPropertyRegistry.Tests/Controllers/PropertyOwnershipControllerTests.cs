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
    public class PropertyOwnershipControllerTests
    {
        private readonly Mock<IPropertyOwnershipService> _mockService;
        private readonly PropertyOwnershipController _controller;

        public PropertyOwnershipControllerTests()
        {
            _mockService = new Mock<IPropertyOwnershipService>();
            _controller = new PropertyOwnershipController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithList()
        {
            // Arrange
            var ownerships = new List<PropertyOwnershipResponseDto>
            {
                TestFixtures.CreatePropertyOwnershipResponseDto(1, 1, 1),
                TestFixtures.CreatePropertyOwnershipResponseDto(2, 2, 2)
            };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(ownerships);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<PropertyOwnershipResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
            _mockService.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnOk()
        {
            // Arrange
            var ownership = TestFixtures.CreatePropertyOwnershipResponseDto(1, 1, 1);
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(ownership);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOwnership = Assert.IsType<PropertyOwnershipResponseDto>(okResult.Value);
            Assert.Equal(1, returnedOwnership.OwnershipId);
            _mockService.Verify(s => s.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((PropertyOwnershipResponseDto?)null);

            // Act
            var result = await _controller.GetById(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
            _mockService.Verify(s => s.GetByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task GetByUser_WithValidUserId_ShouldReturnOk()
        {
            // Arrange
            var userId = 1;
            var ownerships = new List<PropertyOwnershipResponseDto>
            {
                TestFixtures.CreatePropertyOwnershipResponseDto(1, 1, userId),
                TestFixtures.CreatePropertyOwnershipResponseDto(2, 2, userId)
            };
            _mockService.Setup(s => s.GetByUserIdAsync(userId)).ReturnsAsync(ownerships);

            // Act
            var result = await _controller.GetByUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<PropertyOwnershipResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
            _mockService.Verify(s => s.GetByUserIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetByProperty_WithValidPropertyId_ShouldReturnOk()
        {
            // Arrange
            var propertyId = 1;
            var ownerships = new List<PropertyOwnershipResponseDto>
            {
                TestFixtures.CreatePropertyOwnershipResponseDto(1, propertyId, 1),
                TestFixtures.CreatePropertyOwnershipResponseDto(2, propertyId, 2)
            };
            _mockService.Setup(s => s.GetByPropertyIdAsync(propertyId)).ReturnsAsync(ownerships);

            // Act
            var result = await _controller.GetByProperty(propertyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<PropertyOwnershipResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
            _mockService.Verify(s => s.GetByPropertyIdAsync(propertyId), Times.Once);
        }

        [Fact]
        public async Task Create_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var createDto = TestFixtures.CreatePropertyOwnershipCreateDto(1, 1);
            _mockService.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockService.Verify(s => s.CreateAsync(createDto), Times.Once);
        }

        [Fact]
        public async Task Create_WhenFailed_ShouldReturnBadRequest()
        {
            // Arrange
            var createDto = TestFixtures.CreatePropertyOwnershipCreateDto(1, 1);
            _mockService.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult.Value);
            _mockService.Verify(s => s.CreateAsync(createDto), Times.Once);
        }

        [Fact]
        public async Task Verify_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var verifyDto = TestFixtures.CreatePropertyOwnershipVerifyDto(1, 2, true);
            _mockService.Setup(s => s.VerifyOwnershipAsync(verifyDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Verify(verifyDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockService.Verify(s => s.VerifyOwnershipAsync(verifyDto), Times.Once);
        }

        [Fact]
        public async Task Verify_WithInvalidOwnershipId_ShouldReturnNotFound()
        {
            // Arrange
            var verifyDto = TestFixtures.CreatePropertyOwnershipVerifyDto(999, 2, true);
            _mockService.Setup(s => s.VerifyOwnershipAsync(verifyDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.Verify(verifyDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
            _mockService.Verify(s => s.VerifyOwnershipAsync(verifyDto), Times.Once);
        }

        [Fact]
        public async Task Verify_WhenRejecting_ShouldReturnOkWithRejectionMessage()
        {
            // Arrange
            var verifyDto = TestFixtures.CreatePropertyOwnershipVerifyDto(1, 2, false);
            _mockService.Setup(s => s.VerifyOwnershipAsync(verifyDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Verify(verifyDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value!.GetType();
            var messageProperty = response.GetProperty("message");
            Assert.NotNull(messageProperty);
            _mockService.Verify(s => s.VerifyOwnershipAsync(verifyDto), Times.Once);
        }

        [Fact]
        public async Task Delete_WithValidId_ShouldReturnOk()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockService.Verify(s => s.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task Delete_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
            _mockService.Verify(s => s.DeleteAsync(999), Times.Once);
        }
    }
}