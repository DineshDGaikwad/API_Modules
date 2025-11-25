using Xunit;
using Moq;
using APIPropertyRegistry.Services.Implementations;
using APIPropertyRegistry.Repositories.Interfaces;
using APIPropertyRegistry.Models;
using APIPropertyRegistry.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace APIPropertyRegistry.Tests.Services
{
    public class DocumentServiceTests
    {
        private readonly Mock<IDocumentRepository> _mockRepository;
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly DocumentService _service;

        public DocumentServiceTests()
        {
            _mockRepository = new Mock<IDocumentRepository>();
            _mockContext = new Mock<ApplicationDbContext>();
            _service = new DocumentService(_mockRepository.Object, _mockContext.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllDocuments()
        {
            // Arrange
            var documents = new List<Document>
            {
                TestFixtures.CreateTestDocument(1, 1, 1),
                TestFixtures.CreateTestDocument(2, 1, 1)
            };
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(documents);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetPendingAsync_ShouldReturnUnverifiedDocuments()
        {
            // Arrange
            var documents = new List<Document>
            {
                TestFixtures.CreateTestDocument(1, 1, 1, false),
                TestFixtures.CreateTestDocument(2, 1, 1, false)
            };
            _mockRepository.Setup(r => r.GetPendingAsync()).ReturnsAsync(documents);

            // Act
            var result = await _service.GetPendingAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepository.Verify(r => r.GetPendingAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnDocument()
        {
            // Arrange
            var document = TestFixtures.CreateTestDocument(1, 1, 1);
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(document);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.DocumentId);
            _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Document?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldReturnCreatedDocument()
        {
            // Arrange
            var createDto = TestFixtures.CreateDocumentCreateDto(1, 1);
            var uploader = TestFixtures.CreateTestUser(1, "user@test.com", "User");
            var property = TestFixtures.CreateTestProperty(1, 1, 1, true);

            var users = new[] { uploader }.AsQueryable();
            var props = new[] { property }.AsQueryable();

            var mockUsersDbSet = new Mock<IQueryable<User>>();
            var mockPropsDbSet = new Mock<IQueryable<Property>>();

            _mockContext.Setup(c => c.Users).Returns(mockUsersDbSet.Object);
            _mockContext.Setup(c => c.Properties).Returns(mockPropsDbSet.Object);

            var createdDoc = TestFixtures.CreateTestDocument(1, 1, 1);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Document>())).ReturnsAsync(createdDoc);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.DocumentId);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Document>()), Times.Once);
        }

        [Fact]
        public async Task VerifyAsync_WithValidData_ShouldUpdateVerificationStatus()
        {
            // Arrange
            var verifyDto = TestFixtures.CreateDocumentVerifyDto(1, 2, true);
            var document = TestFixtures.CreateTestDocument(1, 1, 1, false);
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(document);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Document>())).ReturnsAsync(true);

            // Act
            var result = await _service.VerifyAsync(verifyDto);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Document>()), Times.Once);
        }

        [Fact]
        public async Task VerifyAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var verifyDto = TestFixtures.CreateDocumentVerifyDto(999, 2, true);
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Document?)null);

            // Act
            var result = await _service.VerifyAsync(verifyDto);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task VerifyAsync_WhenRejecting_ShouldSetVerifiedToFalse()
        {
            // Arrange
            var verifyDto = TestFixtures.CreateDocumentVerifyDto(1, 2, false);
            var document = TestFixtures.CreateTestDocument(1, 1, 1, false);
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(document);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Document>())).ReturnsAsync(true);

            // Act
            var result = await _service.VerifyAsync(verifyDto);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Document>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _service.DeleteAsync(999);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.DeleteAsync(999), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldMapDocumentsToDtos()
        {
            // Arrange
            var documents = new List<Document>
            {
                new Document 
                { 
                    DocumentId = 1, 
                    PropertyId = 1, 
                    UploadedBy = 1,
                    DocumentType = "PropertyDoc",
                    FileName = "doc1.pdf",
                    FilePath = "/path/doc1.pdf",
                    UploadDate = System.DateTime.UtcNow,
                    Verified = false
                }
            };
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(documents);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            var dtoList = result.ToList();
            Assert.Single(dtoList);
            Assert.Equal("PropertyDoc", dtoList[0].DocumentType);
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }
    }
}