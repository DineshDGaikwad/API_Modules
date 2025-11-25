using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APIPropertyRegistry.Controllers;
using APIPropertyRegistry.Services.Interfaces;
using APIPropertyRegistry.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Tests.Controllers
{
    public class DocumentControllerTests
    {
        private readonly Mock<IDocumentService> _mockService;
        private readonly DocumentController _controller;

        public DocumentControllerTests()
        {
            _mockService = new Mock<IDocumentService>();
            _controller = new DocumentController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithList()
        {
            // Arrange
            var documents = new List<DocumentResponseDto>
            {
                TestFixtures.CreateDocumentResponseDto(1, 1, 1),
                TestFixtures.CreateDocumentResponseDto(2, 1, 1)
            };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(documents);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<DocumentResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
            _mockService.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetPending_ShouldReturnOkWithPendingDocuments()
        {
            // Arrange
            var pendingDocs = new List<DocumentResponseDto>
            {
                TestFixtures.CreateDocumentResponseDto(1, 1, 1),
                TestFixtures.CreateDocumentResponseDto(2, 1, 1)
            };
            _mockService.Setup(s => s.GetPendingAsync()).ReturnsAsync(pendingDocs);

            // Act
            var result = await _controller.GetPending();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<DocumentResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
            _mockService.Verify(s => s.GetPendingAsync(), Times.Once);
        }

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnOk()
        {
            // Arrange
            var document = TestFixtures.CreateDocumentResponseDto(1, 1, 1);
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(document);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDoc = Assert.IsType<DocumentResponseDto>(okResult.Value);
            Assert.Equal(1, returnedDoc.DocumentId);
            _mockService.Verify(s => s.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((DocumentResponseDto?)null);

            // Act
            var result = await _controller.GetById(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
            _mockService.Verify(s => s.GetByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task Upload_WithValidDto_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var createDto = TestFixtures.CreateDocumentCreateDto(1, 1);
            var file = TestFixtures.CreateFormFile();
            var responseDto = TestFixtures.CreateDocumentResponseDto(1, 1, 1);
            _mockService.Setup(s => s.CreateAsync(createDto, file)).ReturnsAsync(responseDto);

            // Act
            var result = await _controller.Upload(createDto, file);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(DocumentController.GetById), createdResult.ActionName);
            Assert.Equal(1, ((DocumentResponseDto)createdResult.Value!).DocumentId);
            _mockService.Verify(s => s.CreateAsync(createDto, file), Times.Once);
        }

        [Fact]
        public async Task Upload_WithInvalidModelState_ShouldReturnBadRequest()
        {
            // Arrange
            var createDto = TestFixtures.CreateDocumentCreateDto();
            var file = TestFixtures.CreateFormFile();
            _controller.ModelState.AddModelError("PropertyId", "Required");

            // Act
            var result = await _controller.Upload(createDto, file);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult.Value);
        }

        [Fact]
        public async Task Verify_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var verifyDto = TestFixtures.CreateDocumentVerifyDto(1, 2, true);
            _mockService.Setup(s => s.VerifyAsync(verifyDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Verify(verifyDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockService.Verify(s => s.VerifyAsync(verifyDto), Times.Once);
        }

        [Fact]
        public async Task Verify_WithInvalidDocumentId_ShouldReturnNotFound()
        {
            // Arrange
            var verifyDto = TestFixtures.CreateDocumentVerifyDto(999, 2, true);
            _mockService.Setup(s => s.VerifyAsync(verifyDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.Verify(verifyDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
            _mockService.Verify(s => s.VerifyAsync(verifyDto), Times.Once);
        }

        [Fact]
        public async Task Verify_WhenRejecting_ShouldReturnOkWithRejectionMessage()
        {
            // Arrange
            var verifyDto = TestFixtures.CreateDocumentVerifyDto(1, 2, false);
            _mockService.Setup(s => s.VerifyAsync(verifyDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Verify(verifyDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value!.GetType();
            var messageProperty = response.GetProperty("message");
            Assert.NotNull(messageProperty);
            _mockService.Verify(s => s.VerifyAsync(verifyDto), Times.Once);
        }

        [Fact]
        public async Task Delete_WithValidId_ShouldReturnNoContent()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
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