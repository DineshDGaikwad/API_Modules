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
    public class AdminControllerTests
    {
        private readonly Mock<IAdminService> _mockAdminService;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _mockAdminService = new Mock<IAdminService>();
            _controller = new AdminController(_mockAdminService.Object);
        }

        // ===== AGENT APPROVAL TESTS =====
        [Fact]
        public async Task GetPendingAgents_ShouldReturnOkWithPendingAgentsList()
        {
            // Arrange
            var pendingAgents = new List<AgentApprovalResponseDto>
            {
                TestFixtures.CreateAgentApprovalResponseDto(1, false),
                TestFixtures.CreateAgentApprovalResponseDto(2, false)
            };

            _mockAdminService.Setup(s => s.GetPendingAgentsAsync()).ReturnsAsync(pendingAgents);

            // Act
            var result = await _controller.GetPendingAgents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAgents = Assert.IsAssignableFrom<IEnumerable<AgentApprovalResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedAgents.Count());
            _mockAdminService.Verify(s => s.GetPendingAgentsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetApprovedAgents_ShouldReturnOkWithApprovedAgentsList()
        {
            // Arrange
            var approvedAgents = new List<AgentApprovalResponseDto>
            {
                TestFixtures.CreateAgentApprovalResponseDto(1, true),
                TestFixtures.CreateAgentApprovalResponseDto(2, true)
            };

            _mockAdminService.Setup(s => s.GetApprovedAgentsAsync()).ReturnsAsync(approvedAgents);

            // Act
            var result = await _controller.GetApprovedAgents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAgents = Assert.IsAssignableFrom<IEnumerable<AgentApprovalResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedAgents.Count());
            _mockAdminService.Verify(s => s.GetApprovedAgentsAsync(), Times.Once);
        }

        [Fact]
        public async Task ApproveOrRejectAgent_WithApproval_ShouldReturnOk()
        {
            // Arrange
            var agentId = 1;
            var approvalDto = TestFixtures.CreateApproveAgentDto(true);
            _mockAdminService.Setup(s => s.ApproveOrRejectAgentAsync(agentId, true, approvalDto.Remarks))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ApproveOrRejectAgent(agentId, approvalDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockAdminService.Verify(s => s.ApproveOrRejectAgentAsync(agentId, true, approvalDto.Remarks), Times.Once);
        }

        [Fact]
        public async Task ApproveOrRejectAgent_WithRejection_ShouldReturnOk()
        {
            // Arrange
            var agentId = 1;
            var approvalDto = TestFixtures.CreateApproveAgentDto(false);
            _mockAdminService.Setup(s => s.ApproveOrRejectAgentAsync(agentId, false, approvalDto.Remarks))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ApproveOrRejectAgent(agentId, approvalDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task ApproveOrRejectAgent_WithInvalidAgentId_ShouldReturnNotFound()
        {
            // Arrange
            var agentId = 999;
            var approvalDto = TestFixtures.CreateApproveAgentDto(true);
            _mockAdminService.Setup(s => s.ApproveOrRejectAgentAsync(agentId, true, approvalDto.Remarks))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.ApproveOrRejectAgent(agentId, approvalDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
        }

        [Fact]
        public async Task ApproveOrRejectAgent_WithNullDto_ShouldReturnBadRequest()
        {
            // Arrange
            var agentId = 1;

            // Act
            var result = await _controller.ApproveOrRejectAgent(agentId, null!);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult.Value);
        }

        // ===== PROPERTY APPROVAL TESTS =====
        [Fact]
        public async Task GetPendingProperties_ShouldReturnOkWithPendingPropertiesList()
        {
            // Arrange
            var pendingProperties = new List<PropertyApprovalResponseDto>
            {
                TestFixtures.CreatePropertyApprovalResponseDto(1, false),
                TestFixtures.CreatePropertyApprovalResponseDto(2, false)
            };

            _mockAdminService.Setup(s => s.GetPendingPropertiesAsync()).ReturnsAsync(pendingProperties);

            // Act
            var result = await _controller.GetPendingProperties();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProperties = Assert.IsAssignableFrom<IEnumerable<PropertyApprovalResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedProperties.Count());
            _mockAdminService.Verify(s => s.GetPendingPropertiesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetApprovedProperties_ShouldReturnOkWithApprovedPropertiesList()
        {
            // Arrange
            var approvedProperties = new List<PropertyApprovalResponseDto>
            {
                TestFixtures.CreatePropertyApprovalResponseDto(1, true),
                TestFixtures.CreatePropertyApprovalResponseDto(2, true)
            };

            _mockAdminService.Setup(s => s.GetApprovedPropertiesAsync()).ReturnsAsync(approvedProperties);

            // Act
            var result = await _controller.GetApprovedProperties();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProperties = Assert.IsAssignableFrom<IEnumerable<PropertyApprovalResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedProperties.Count());
            _mockAdminService.Verify(s => s.GetApprovedPropertiesAsync(), Times.Once);
        }

        [Fact]
        public async Task ApproveOrRejectProperty_WithApproval_ShouldReturnOk()
        {
            // Arrange
            var propertyId = 1;
            var adminId = 1;
            var approvalDto = TestFixtures.CreateApprovePropertyDto(true);
            _mockAdminService.Setup(s => s.ApproveOrRejectPropertyAsync(propertyId, adminId, true, approvalDto.Remarks))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ApproveOrRejectProperty(propertyId, adminId, approvalDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockAdminService.Verify(s => s.ApproveOrRejectPropertyAsync(propertyId, adminId, true, approvalDto.Remarks), Times.Once);
        }

        [Fact]
        public async Task ApproveOrRejectProperty_WithRejection_ShouldReturnOk()
        {
            // Arrange
            var propertyId = 1;
            var adminId = 1;
            var approvalDto = TestFixtures.CreateApprovePropertyDto(false);
            _mockAdminService.Setup(s => s.ApproveOrRejectPropertyAsync(propertyId, adminId, false, approvalDto.Remarks))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ApproveOrRejectProperty(propertyId, adminId, approvalDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task ApproveOrRejectProperty_WithInvalidPropertyId_ShouldReturnNotFound()
        {
            // Arrange
            var propertyId = 999;
            var adminId = 1;
            var approvalDto = TestFixtures.CreateApprovePropertyDto(true);
            _mockAdminService.Setup(s => s.ApproveOrRejectPropertyAsync(propertyId, adminId, true, approvalDto.Remarks))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.ApproveOrRejectProperty(propertyId, adminId, approvalDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
        }

        [Fact]
        public async Task ApproveOrRejectProperty_WithNullDto_ShouldReturnBadRequest()
        {
            // Arrange
            var propertyId = 1;
            var adminId = 1;

            // Act
            var result = await _controller.ApproveOrRejectProperty(propertyId, adminId, null!);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult.Value);
        }
    }
}