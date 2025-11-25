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
    public class AdminServiceTests
    {
        private readonly Mock<IAdminRepository> _mockRepository;
        private readonly Mock<IPropertyRepository> _mockPropertyRepository;
        private readonly AdminService _service;

        public AdminServiceTests()
        {
            _mockRepository = new Mock<IAdminRepository>();
            _mockPropertyRepository = new Mock<IPropertyRepository>();
            _mockPropertyRepository.Setup(r => r.PropertyNumberExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _service = new AdminService(_mockRepository.Object, _mockPropertyRepository.Object, new PropertyNumberGeneratorService());
        }

        // ===== GET PENDING AGENTS TESTS =====
        [Fact]
        public async Task GetPendingAgentsAsync_ShouldReturnPendingAgents()
        {
            // Arrange
            var pendingAgents = new List<User>
            {
                TestFixtures.CreateTestUser(1, "agent1@test.com", "Agent"),
                TestFixtures.CreateTestUser(2, "agent2@test.com", "Agent")
            };

            _mockRepository.Setup(r => r.GetPendingAgentsAsync()).ReturnsAsync(pendingAgents);

            // Act
            var result = await _service.GetPendingAgentsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepository.Verify(r => r.GetPendingAgentsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetPendingAgentsAsync_WhenNoPendingAgents_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyList = new List<User>();
            _mockRepository.Setup(r => r.GetPendingAgentsAsync()).ReturnsAsync(emptyList);

            // Act
            var result = await _service.GetPendingAgentsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        // ===== GET APPROVED AGENTS TESTS =====
        [Fact]
        public async Task GetApprovedAgentsAsync_ShouldReturnApprovedAgents()
        {
            // Arrange
            var approvedAgents = new List<User>
            {
                TestFixtures.CreateTestUser(1, "agent1@test.com", "Agent"),
                TestFixtures.CreateTestUser(2, "agent2@test.com", "Agent")
            };
            approvedAgents.ForEach(a => a.IsApproved = true);

            _mockRepository.Setup(r => r.GetApprovedAgentsAsync()).ReturnsAsync(approvedAgents);

            // Act
            var result = await _service.GetApprovedAgentsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepository.Verify(r => r.GetApprovedAgentsAsync(), Times.Once);
        }

        // ===== APPROVE/REJECT AGENT TESTS =====
        [Fact]
        public async Task ApproveOrRejectAgentAsync_WithApproval_ShouldReturnTrue()
        {
            // Arrange
            var agentId = 1;
            var agent = TestFixtures.CreateTestUser(agentId, "agent@test.com", "Agent");
            agent.IsApproved = false;

            _mockRepository.Setup(r => r.GetAgentByIdAsync(agentId)).ReturnsAsync(agent);
            _mockRepository.Setup(r => r.UpdateAgentAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.ApproveOrRejectAgentAsync(agentId, true, "Agent looks good");

            // Assert
            Assert.True(result);
            Assert.True(agent.IsApproved);
            Assert.Equal("Agent looks good", agent.Remarks);
            _mockRepository.Verify(r => r.UpdateAgentAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task ApproveOrRejectAgentAsync_WithRejection_ShouldReturnTrue()
        {
            // Arrange
            var agentId = 1;
            var agent = TestFixtures.CreateTestUser(agentId, "agent@test.com", "Agent");
            agent.IsApproved = false;

            _mockRepository.Setup(r => r.GetAgentByIdAsync(agentId)).ReturnsAsync(agent);
            _mockRepository.Setup(r => r.UpdateAgentAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.ApproveOrRejectAgentAsync(agentId, false, "Does not meet requirements");

            // Assert
            Assert.True(result);
            Assert.False(agent.IsApproved);
            Assert.Equal("Does not meet requirements", agent.Remarks);
        }

        [Fact]
        public async Task ApproveOrRejectAgentAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var agentId = 999;
            _mockRepository.Setup(r => r.GetAgentByIdAsync(agentId)).ReturnsAsync((User?)null);

            // Act
            var result = await _service.ApproveOrRejectAgentAsync(agentId, true, "Remarks");

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.UpdateAgentAsync(It.IsAny<User>()), Times.Never);
        }

        // ===== GET PENDING PROPERTIES TESTS =====
        [Fact]
        public async Task GetPendingPropertiesAsync_ShouldReturnPendingProperties()
        {
            // Arrange
            var pendingProperties = new List<Property>
            {
                TestFixtures.CreateTestProperty(1, 1, 1, false),
                TestFixtures.CreateTestProperty(2, 2, 1, false)
            };

            _mockRepository.Setup(r => r.GetPendingPropertiesAsync()).ReturnsAsync(pendingProperties);

            // Act
            var result = await _service.GetPendingPropertiesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepository.Verify(r => r.GetPendingPropertiesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetPendingPropertiesAsync_WhenNoProperties_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyList = new List<Property>();
            _mockRepository.Setup(r => r.GetPendingPropertiesAsync()).ReturnsAsync(emptyList);

            // Act
            var result = await _service.GetPendingPropertiesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        // ===== GET APPROVED PROPERTIES TESTS =====
        [Fact]
        public async Task GetApprovedPropertiesAsync_ShouldReturnApprovedProperties()
        {
            // Arrange
            var approvedProperties = new List<Property>
            {
                TestFixtures.CreateTestProperty(1, 1, 1, true),
                TestFixtures.CreateTestProperty(2, 2, 1, true)
            };

            _mockRepository.Setup(r => r.GetApprovedPropertiesAsync()).ReturnsAsync(approvedProperties);

            // Act
            var result = await _service.GetApprovedPropertiesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepository.Verify(r => r.GetApprovedPropertiesAsync(), Times.Once);
        }

        // ===== APPROVE/REJECT PROPERTY TESTS =====
        [Fact]
        public async Task ApproveOrRejectPropertyAsync_WithApproval_ShouldReturnTrue()
        {
            // Arrange
            var propertyId = 1;
            var adminId = 1;
            var property = TestFixtures.CreateTestProperty(propertyId, 1, 1, false);

            _mockRepository.Setup(r => r.GetPropertyByIdAsync(propertyId)).ReturnsAsync(property);
            _mockRepository.Setup(r => r.UpdatePropertyAsync(It.IsAny<Property>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.ApproveOrRejectPropertyAsync(propertyId, adminId, true, "Looks good");

            // Assert
            Assert.True(result);
            Assert.True(property.IsApproved);
            Assert.Equal("Approved", property.Status);
            Assert.Equal(adminId, property.ApprovedBy);
            Assert.Equal("Looks good", property.Remarks);
            _mockRepository.Verify(r => r.UpdatePropertyAsync(It.IsAny<Property>()), Times.Once);
        }

        [Fact]
        public async Task ApproveOrRejectPropertyAsync_WithRejection_ShouldReturnTrue()
        {
            // Arrange
            var propertyId = 1;
            var adminId = 1;
            var property = TestFixtures.CreateTestProperty(propertyId);

            _mockRepository.Setup(r => r.GetPropertyByIdAsync(propertyId)).ReturnsAsync(property);
            _mockRepository.Setup(r => r.UpdatePropertyAsync(It.IsAny<Property>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.ApproveOrRejectPropertyAsync(propertyId, adminId, false, "Needs revision");

            // Assert
            Assert.True(result);
            Assert.False(property.IsApproved);
            Assert.Equal("Rejected", property.Status);
            Assert.Equal("Needs revision", property.Remarks);
        }

        [Fact]
        public async Task ApproveOrRejectPropertyAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var propertyId = 999;
            var adminId = 1;
            _mockRepository.Setup(r => r.GetPropertyByIdAsync(propertyId)).ReturnsAsync((Property?)null);

            // Act
            var result = await _service.ApproveOrRejectPropertyAsync(propertyId, adminId, true, "Remarks");

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.UpdatePropertyAsync(It.IsAny<Property>()), Times.Never);
        }

        [Fact]
        public async Task ApproveOrRejectPropertyAsync_ShouldUpdateTimestamp()
        {
            // Arrange
            var propertyId = 1;
            var adminId = 1;
            var property = TestFixtures.CreateTestProperty(propertyId);
            property.UpdatedAt = null;

            _mockRepository.Setup(r => r.GetPropertyByIdAsync(propertyId)).ReturnsAsync(property);
            _mockRepository.Setup(r => r.UpdatePropertyAsync(It.IsAny<Property>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.ApproveOrRejectPropertyAsync(propertyId, adminId, true, "Remarks");

            // Assert
            Assert.True(result);
            Assert.NotNull(property.UpdatedAt);
        }
    }
}