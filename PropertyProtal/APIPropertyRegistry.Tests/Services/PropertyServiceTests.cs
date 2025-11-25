using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using APIPropertyRegistry.Services.Implementations;
using APIPropertyRegistry.Repositories.Interfaces;
using APIPropertyRegistry.Services.Interfaces;
using APIPropertyRegistry.Models;
using APIPropertyRegistry.DTOs.PropertyDtos;
using APIPropertyRegistry.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Tests.Services
{
    public class PropertyServiceTests
    {
        private readonly Mock<IPropertyRepository> _mockRepository;
        private readonly Mock<IDocumentService> _mockDocumentService;
        private readonly Mock<IAgentPropertyRepository> _mockAgentPropertyRepository;
        private readonly Mock<IPropertyOwnershipRepository> _mockOwnershipRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly PropertyService _service;

        public PropertyServiceTests()
        {
            _mockRepository = new Mock<IPropertyRepository>();
            _mockDocumentService = new Mock<IDocumentService>();
            _mockAgentPropertyRepository = new Mock<IAgentPropertyRepository>();
            _mockOwnershipRepository = new Mock<IPropertyOwnershipRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockRepository.Setup(r => r.PropertyNumberExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _service = new PropertyService(
                _mockRepository.Object,
                _mockDocumentService.Object,
                _mockAgentPropertyRepository.Object,
                _mockOwnershipRepository.Object,
                new PropertyNumberGeneratorService(),
                _mockUserRepository.Object
            );
        }

        // ===== GET ALL TESTS =====
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProperties()
        {
            // Arrange
            var properties = TestFixtures.CreateTestPropertyList(3);
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(properties);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoProperties_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyList = new List<Property>();
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(emptyList);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        // ===== GET BY ID TESTS =====
        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnProperty()
        {
            // Arrange
            var propertyId = 1;
            var property = TestFixtures.CreateTestProperty(propertyId);
            _mockRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(property);

            // Act
            var result = await _service.GetByIdAsync(propertyId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(propertyId, result.PropertyId);
            Assert.Equal("Test Property 1", result.Title);
            _mockRepository.Verify(r => r.GetByIdAsync(propertyId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var propertyId = 999;
            _mockRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync((Property?)null);

            // Act
            var result = await _service.GetByIdAsync(propertyId);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.GetByIdAsync(propertyId), Times.Once);
        }

        // ===== GET BY OWNER ID TESTS =====
        [Fact]
        public async Task GetByOwnerIdAsync_WithValidOwnerId_ShouldReturnPropertiesByOwner()
        {
            // Arrange
            var ownerId = 1;
            var properties = new List<Property>
            {
                TestFixtures.CreateTestProperty(1, ownerId),
                TestFixtures.CreateTestProperty(2, ownerId)
            };

            _mockRepository.Setup(r => r.GetByOwnerIdAsync(ownerId)).ReturnsAsync(properties);

            // Act
            var result = await _service.GetByOwnerIdAsync(ownerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepository.Verify(r => r.GetByOwnerIdAsync(ownerId), Times.Once);
        }

        // ===== GET PENDING TESTS =====
        [Fact]
        public async Task GetPendingAsync_ShouldReturnPendingProperties()
        {
            // Arrange
            var pendingProperties = new List<Property>
            {
                TestFixtures.CreateTestProperty(1, 1, 1, false),
                TestFixtures.CreateTestProperty(2, 2, 1, false)
            };

            _mockRepository.Setup(r => r.GetPendingAsync()).ReturnsAsync(pendingProperties);

            // Act
            var result = await _service.GetPendingAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.Equal("Pending", p.Status));
            _mockRepository.Verify(r => r.GetPendingAsync(), Times.Once);
        }

        // ===== CREATE TESTS =====
        [Fact]
        public async Task CreateAsync_WithValidDto_ShouldReturnPropertyResponse()
        {
            // Arrange
            var createDto = TestFixtures.CreatePropertyCreateDto();
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Property>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
            _mockDocumentService
                .Setup(s => s.CreateAsync(It.IsAny<DocumentCreateDto>(), It.IsAny<IFormFile>()))
                .ReturnsAsync(TestFixtures.CreateDocumentResponseDto());

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Pending", result!.Status);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Property>()), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockDocumentService.Verify(s => s.CreateAsync(It.IsAny<DocumentCreateDto>(), It.IsAny<IFormFile>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenSaveFails_ShouldThrowException()
        {
            // Arrange
            var createDto = TestFixtures.CreatePropertyCreateDto();
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Property>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(createDto));
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockDocumentService.Verify(s => s.CreateAsync(It.IsAny<DocumentCreateDto>(), It.IsAny<IFormFile>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ShouldSetPropertyAsNotApproved()
        {
            // Arrange
            var createDto = TestFixtures.CreatePropertyCreateDto();
            Property? capturedProperty = null;

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Property>()))
                .Callback<Property>(p => capturedProperty = p)
                .Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
            _mockDocumentService
                .Setup(s => s.CreateAsync(It.IsAny<DocumentCreateDto>(), It.IsAny<IFormFile>()))
                .ReturnsAsync(TestFixtures.CreateDocumentResponseDto());

            // Act
            await _service.CreateAsync(createDto);

            // Assert
            Assert.NotNull(capturedProperty);
            Assert.False(capturedProperty!.IsApproved);
            Assert.Equal("Pending", capturedProperty.Status);
        }

        // ===== UPDATE TESTS =====
        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldReturnTrue()
        {
            // Arrange
            var propertyId = 1;
            var property = TestFixtures.CreateTestProperty(propertyId);
            var updateDto = TestFixtures.CreatePropertyUpdateDto();

            _mockRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(property);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Property>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _service.UpdateAsync(propertyId, updateDto);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Property>()), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var propertyId = 999;
            var updateDto = TestFixtures.CreatePropertyUpdateDto();
            _mockRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync((Property?)null);

            // Act
            var result = await _service.UpdateAsync(propertyId, updateDto);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Property>()), Times.Never);
        }

        // ===== DELETE TESTS =====
        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            var propertyId = 1;
            _mockRepository.Setup(r => r.DeleteAsync(propertyId)).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(propertyId);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.DeleteAsync(propertyId), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenSaveFails_ShouldReturnFalse()
        {
            // Arrange
            var propertyId = 1;
            _mockRepository.Setup(r => r.DeleteAsync(propertyId)).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(false);

            // Act
            var result = await _service.DeleteAsync(propertyId);

            // Assert
            Assert.False(result);
        }

        // ===== APPROVE TESTS =====
        [Fact]
        public async Task ApprovePropertyAsync_WithApproval_ShouldReturnTrue()
        {
            // Arrange
            var propertyId = 1;
            var adminId = 1;
            var property = TestFixtures.CreateTestProperty(propertyId, 1, 1, false);
            Property? capturedProperty = null;

            _mockRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(property);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Property>()))
                .Callback<Property>(p => capturedProperty = p)
                .Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _service.ApprovePropertyAsync(propertyId, true, adminId, "Looks good");

            // Assert
            Assert.True(result);
            Assert.NotNull(capturedProperty);
            Assert.True(capturedProperty.IsApproved);
            Assert.Equal("Approved", capturedProperty.Status);
            Assert.Equal(adminId, capturedProperty.ApprovedBy);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Property>()), Times.Once);
        }

        [Fact]
        public async Task ApprovePropertyAsync_WithRejection_ShouldReturnTrue()
        {
            // Arrange
            var propertyId = 1;
            var adminId = 1;
            var property = TestFixtures.CreateTestProperty(propertyId);
            Property? capturedProperty = null;

            _mockRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(property);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Property>()))
                .Callback<Property>(p => capturedProperty = p)
                .Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _service.ApprovePropertyAsync(propertyId, false, adminId, "Needs more info");

            // Assert
            Assert.True(result);
            Assert.NotNull(capturedProperty);
            Assert.False(capturedProperty.IsApproved);
            Assert.Equal("Rejected", capturedProperty.Status);
        }

        [Fact]
        public async Task ApprovePropertyAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var propertyId = 999;
            var adminId = 1;
            _mockRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync((Property?)null);

            // Act
            var result = await _service.ApprovePropertyAsync(propertyId, true, adminId, "Looks good");

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Property>()), Times.Never);
        }
    }
}