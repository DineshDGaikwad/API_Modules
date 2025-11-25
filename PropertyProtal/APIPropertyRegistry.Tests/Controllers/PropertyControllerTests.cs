using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using APIPropertyRegistry.Controllers;
using APIPropertyRegistry.Services.Interfaces;
using APIPropertyRegistry.DTOs.PropertyDtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace APIPropertyRegistry.Tests.Controllers
{
    public class PropertyControllerTests
    {
        private readonly Mock<IPropertyService> _mockPropertyService;
        private readonly PropertyController _controller;

        public PropertyControllerTests()
        {
            _mockPropertyService = new Mock<IPropertyService>();
            _controller = new PropertyController(_mockPropertyService.Object);
        }

        // ===== GET ALL TESTS =====
        [Fact]
        public async Task GetAll_ShouldReturnOkWithPropertyList()
        {
            // Arrange
            var properties = new List<PropertyResponseDto>
            {
                TestFixtures.CreatePropertyResponseDto(1),
                TestFixtures.CreatePropertyResponseDto(2)
            };

            _mockPropertyService.Setup(s => s.GetAllAsync()).ReturnsAsync(properties);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProperties = Assert.IsAssignableFrom<IEnumerable<PropertyResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedProperties.Count());
            _mockPropertyService.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenNoProperties_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyList = new List<PropertyResponseDto>();
            _mockPropertyService.Setup(s => s.GetAllAsync()).ReturnsAsync(emptyList);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProperties = Assert.IsAssignableFrom<IEnumerable<PropertyResponseDto>>(okResult.Value);
            Assert.Empty(returnedProperties);
        }

        // ===== GET BY ID TESTS =====
        [Fact]
        public async Task GetById_WithValidId_ShouldReturnOkWithProperty()
        {
            // Arrange
            var propertyId = 1;
            var property = TestFixtures.CreatePropertyResponseDto(propertyId);
            _mockPropertyService.Setup(s => s.GetByIdAsync(propertyId)).ReturnsAsync(property);

            // Act
            var result = await _controller.GetById(propertyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProperty = Assert.IsType<PropertyResponseDto>(okResult.Value);
            Assert.Equal(propertyId, returnedProperty.PropertyId);
            _mockPropertyService.Verify(s => s.GetByIdAsync(propertyId), Times.Once);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var propertyId = 999;
            _mockPropertyService.Setup(s => s.GetByIdAsync(propertyId)).ReturnsAsync((PropertyResponseDto?)null);

            // Act
            var result = await _controller.GetById(propertyId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
            _mockPropertyService.Verify(s => s.GetByIdAsync(propertyId), Times.Once);
        }

        // ===== GET BY OWNER ID TESTS =====
        [Fact]
        public async Task GetByOwnerId_WithValidOwnerId_ShouldReturnOkWithProperties()
        {
            // Arrange
            var ownerId = 1;
            var properties = new List<PropertyResponseDto>
            {
                TestFixtures.CreatePropertyResponseDto(1, ownerId),
                TestFixtures.CreatePropertyResponseDto(2, ownerId)
            };

            _mockPropertyService.Setup(s => s.GetByOwnerIdAsync(ownerId)).ReturnsAsync(properties);

            // Act
            var result = await _controller.GetByOwnerId(ownerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProperties = Assert.IsAssignableFrom<IEnumerable<PropertyResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedProperties.Count());
            _mockPropertyService.Verify(s => s.GetByOwnerIdAsync(ownerId), Times.Once);
        }

        // ===== GET PENDING TESTS =====
        [Fact]
        public async Task GetPending_ShouldReturnOkWithPendingProperties()
        {
            // Arrange
            var pendingProperties = new List<PropertyResponseDto>
            {
                TestFixtures.CreatePropertyResponseDto(1, 1)
            };

            _mockPropertyService.Setup(s => s.GetPendingAsync()).ReturnsAsync(pendingProperties);

            // Act
            var result = await _controller.GetPending();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProperties = Assert.IsAssignableFrom<IEnumerable<PropertyResponseDto>>(okResult.Value);
            Assert.Single(returnedProperties);
            _mockPropertyService.Verify(s => s.GetPendingAsync(), Times.Once);
        }

        // ===== CREATE TESTS =====
        [Fact]
        public async Task Create_WithValidDto_ShouldReturnOk()
        {
            // Arrange
            var createDto = TestFixtures.CreatePropertyCreateDto();
            _mockPropertyService.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockPropertyService.Verify(s => s.CreateAsync(createDto), Times.Once);
        }

        [Fact]
        public async Task Create_WhenCreationFails_ShouldReturnBadRequest()
        {
            // Arrange
            var createDto = TestFixtures.CreatePropertyCreateDto();
            _mockPropertyService.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult.Value);
            _mockPropertyService.Verify(s => s.CreateAsync(createDto), Times.Once);
        }

        // ===== UPDATE TESTS =====
        [Fact]
        public async Task Update_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var propertyId = 1;
            var updateDto = TestFixtures.CreatePropertyUpdateDto();
            _mockPropertyService.Setup(s => s.UpdateAsync(propertyId, updateDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Update(propertyId, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockPropertyService.Verify(s => s.UpdateAsync(propertyId, updateDto), Times.Once);
        }

        [Fact]
        public async Task Update_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var propertyId = 999;
            var updateDto = TestFixtures.CreatePropertyUpdateDto();
            _mockPropertyService.Setup(s => s.UpdateAsync(propertyId, updateDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.Update(propertyId, updateDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
            _mockPropertyService.Verify(s => s.UpdateAsync(propertyId, updateDto), Times.Once);
        }

        // ===== DELETE TESTS =====
        [Fact]
        public async Task Delete_WithValidId_ShouldReturnOk()
        {
            // Arrange
            var propertyId = 1;
            _mockPropertyService.Setup(s => s.DeleteAsync(propertyId)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(propertyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockPropertyService.Verify(s => s.DeleteAsync(propertyId), Times.Once);
        }

        [Fact]
        public async Task Delete_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var propertyId = 999;
            _mockPropertyService.Setup(s => s.DeleteAsync(propertyId)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(propertyId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
            _mockPropertyService.Verify(s => s.DeleteAsync(propertyId), Times.Once);
        }

        // ===== APPROVE TESTS =====
        [Fact]
        public async Task Approve_WithApprovalRequest_ShouldReturnOk()
        {
            // Arrange
            var approvalDto = TestFixtures.CreatePropertyApprovalDto(1, true, 1);
            _mockPropertyService.Setup(s => s.ApprovePropertyAsync(
                approvalDto.PropertyId, 
                approvalDto.Approve, 
                approvalDto.AdminId, 
                approvalDto.Remarks))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Approve(approvalDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockPropertyService.Verify(s => s.ApprovePropertyAsync(
                approvalDto.PropertyId, 
                approvalDto.Approve, 
                approvalDto.AdminId, 
                approvalDto.Remarks), 
                Times.Once);
        }

        [Fact]
        public async Task Approve_WithRejectionRequest_ShouldReturnOk()
        {
            // Arrange
            var approvalDto = TestFixtures.CreatePropertyApprovalDto(1, false, 1);
            _mockPropertyService.Setup(s => s.ApprovePropertyAsync(
                approvalDto.PropertyId, 
                approvalDto.Approve, 
                approvalDto.AdminId, 
                approvalDto.Remarks))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Approve(approvalDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockPropertyService.Verify(s => s.ApprovePropertyAsync(
                approvalDto.PropertyId, 
                approvalDto.Approve, 
                approvalDto.AdminId, 
                approvalDto.Remarks), 
                Times.Once);
        }

        [Fact]
        public async Task Approve_WithInvalidPropertyId_ShouldReturnNotFound()
        {
            // Arrange
            var approvalDto = TestFixtures.CreatePropertyApprovalDto(999, true, 1);
            _mockPropertyService.Setup(s => s.ApprovePropertyAsync(
                approvalDto.PropertyId, 
                approvalDto.Approve, 
                approvalDto.AdminId, 
                approvalDto.Remarks))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Approve(approvalDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
        }
    }
}