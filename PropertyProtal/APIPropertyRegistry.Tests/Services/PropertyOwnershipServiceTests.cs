using Xunit;
using Moq;
using APIPropertyRegistry.Services.Implementations;
using APIPropertyRegistry.Repositories.Interfaces;
using APIPropertyRegistry.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace APIPropertyRegistry.Tests.Services
{
    public class PropertyOwnershipServiceTests
    {
        private readonly Mock<IPropertyOwnershipRepository> _mockRepository;
        private readonly PropertyOwnershipService _service;

        public PropertyOwnershipServiceTests()
        {
            _mockRepository = new Mock<IPropertyOwnershipRepository>();
            _service = new PropertyOwnershipService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllOwnerships()
        {
            // Arrange
            var ownerships = new List<PropertyOwnership>
            {
                TestFixtures.CreateTestPropertyOwnership(1, 1, 1),
                TestFixtures.CreateTestPropertyOwnership(2, 2, 2)
            };
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(ownerships);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnOwnership()
        {
            // Arrange
            var ownership = TestFixtures.CreateTestPropertyOwnership(1, 1, 1);
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ownership);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.OwnershipId);
            _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((PropertyOwnership?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnUserOwnerships()
        {
            // Arrange
            var userId = 1;
            var ownerships = new List<PropertyOwnership>
            {
                TestFixtures.CreateTestPropertyOwnership(1, 1, userId),
                TestFixtures.CreateTestPropertyOwnership(2, 2, userId)
            };
            _mockRepository.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(ownerships);

            // Act
            var result = await _service.GetByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepository.Verify(r => r.GetByUserIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetByPropertyIdAsync_ShouldReturnPropertyOwnerships()
        {
            // Arrange
            var propertyId = 1;
            var ownerships = new List<PropertyOwnership>
            {
                TestFixtures.CreateTestPropertyOwnership(1, propertyId, 1),
                TestFixtures.CreateTestPropertyOwnership(2, propertyId, 2)
            };
            _mockRepository.Setup(r => r.GetByPropertyIdAsync(propertyId)).ReturnsAsync(ownerships);

            // Act
            var result = await _service.GetByPropertyIdAsync(propertyId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepository.Verify(r => r.GetByPropertyIdAsync(propertyId), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldReturnTrue()
        {
            // Arrange
            var createDto = TestFixtures.CreatePropertyOwnershipCreateDto(1, 1);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<PropertyOwnership>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<PropertyOwnership>()), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenSaveFails_ShouldReturnFalse()
        {
            // Arrange
            var createDto = TestFixtures.CreatePropertyOwnershipCreateDto(1, 1);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<PropertyOwnership>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(false);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task VerifyOwnershipAsync_WithValidData_ShouldReturnTrue()
        {
            // Arrange
            var verifyDto = TestFixtures.CreatePropertyOwnershipVerifyDto(1, 2, true);
            var ownership = TestFixtures.CreateTestPropertyOwnership(1, 1, 1, false);
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ownership);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<PropertyOwnership>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _service.VerifyOwnershipAsync(verifyDto);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<PropertyOwnership>()), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task VerifyOwnershipAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var verifyDto = TestFixtures.CreatePropertyOwnershipVerifyDto(999, 2, true);
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((PropertyOwnership?)null);

            // Act
            var result = await _service.VerifyOwnershipAsync(verifyDto);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task VerifyOwnershipAsync_WhenApproving_ShouldSetStatusToApproved()
        {
            // Arrange
            var verifyDto = TestFixtures.CreatePropertyOwnershipVerifyDto(1, 2, true);
            var ownership = TestFixtures.CreateTestPropertyOwnership(1, 1, 1, false);
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ownership);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<PropertyOwnership>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _service.VerifyOwnershipAsync(verifyDto);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task VerifyOwnershipAsync_WhenRejecting_ShouldSetStatusToRejected()
        {
            // Arrange
            var verifyDto = TestFixtures.CreatePropertyOwnershipVerifyDto(1, 2, false);
            var ownership = TestFixtures.CreateTestPropertyOwnership(1, 1, 1, false);
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ownership);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<PropertyOwnership>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _service.VerifyOwnershipAsync(verifyDto);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<PropertyOwnership>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenSaveFails_ShouldReturnFalse()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(false);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}