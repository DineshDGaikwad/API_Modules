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
    public class AgentPropertyControllerTests
    {
        private readonly Mock<IAgentPropertyService> _mockService;
        private readonly AgentPropertyController _controller;

        public AgentPropertyControllerTests()
        {
            _mockService = new Mock<IAgentPropertyService>();
            _controller = new AgentPropertyController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithList()
        {
            // Arrange
            var assignments = new List<AgentPropertyResponseDto>
            {
                TestFixtures.CreateAgentPropertyResponseDto(1, 1, 1),
                TestFixtures.CreateAgentPropertyResponseDto(2, 2, 2)
            };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(assignments);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<AgentPropertyResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
            _mockService.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnOk()
        {
            // Arrange
            var assignment = TestFixtures.CreateAgentPropertyResponseDto(1, 1, 1);
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(assignment);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAssignment = Assert.IsType<AgentPropertyResponseDto>(okResult.Value);
            Assert.Equal(1, returnedAssignment.AgentPropertyId);
            _mockService.Verify(s => s.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((AgentPropertyResponseDto?)null);

            // Act
            var result = await _controller.GetById(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
            _mockService.Verify(s => s.GetByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task GetByAgent_WithValidAgentId_ShouldReturnOk()
        {
            // Arrange
            var agentId = 1;
            var assignments = new List<AgentPropertyResponseDto>
            {
                TestFixtures.CreateAgentPropertyResponseDto(1, agentId, 1),
                TestFixtures.CreateAgentPropertyResponseDto(2, agentId, 2)
            };
            _mockService.Setup(s => s.GetByAgentAsync(agentId)).ReturnsAsync(assignments);

            // Act
            var result = await _controller.GetByAgent(agentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<AgentPropertyResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
            _mockService.Verify(s => s.GetByAgentAsync(agentId), Times.Once);
        }

        [Fact]
        public async Task Create_WithValidDto_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var createDto = TestFixtures.CreateAgentPropertyCreateDto(1, 1);
            var responseDto = TestFixtures.CreateAgentPropertyResponseDto(1, 1, 1);
            _mockService.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(responseDto);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(AgentPropertyController.GetById), createdResult.ActionName);
            Assert.Equal(1, ((AgentPropertyResponseDto)createdResult.Value!).AgentPropertyId);
            _mockService.Verify(s => s.CreateAsync(createDto), Times.Once);
        }

        [Fact]
        public async Task Create_WithInvalidModelState_ShouldReturnBadRequest()
        {
            // Arrange
            var createDto = TestFixtures.CreateAgentPropertyCreateDto();
            _controller.ModelState.AddModelError("AgentId", "Required");

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult.Value);
        }

        [Fact]
        public async Task Approve_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var approveDto = TestFixtures.CreateAgentPropertyApproveDto(1, true);
            _mockService.Setup(s => s.ApproveAsync(approveDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Approve(approveDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockService.Verify(s => s.ApproveAsync(approveDto), Times.Once);
        }

        [Fact]
        public async Task Approve_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var approveDto = TestFixtures.CreateAgentPropertyApproveDto(999, true);
            _mockService.Setup(s => s.ApproveAsync(approveDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.Approve(approveDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
            _mockService.Verify(s => s.ApproveAsync(approveDto), Times.Once);
        }

        [Fact]
        public async Task Approve_WhenRevoking_ShouldReturnOkWithRevokingMessage()
        {
            // Arrange
            var approveDto = TestFixtures.CreateAgentPropertyApproveDto(1, false);
            _mockService.Setup(s => s.ApproveAsync(approveDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Approve(approveDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value!.GetType();
            var messageProperty = response.GetProperty("message");
            Assert.NotNull(messageProperty);
            _mockService.Verify(s => s.ApproveAsync(approveDto), Times.Once);
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