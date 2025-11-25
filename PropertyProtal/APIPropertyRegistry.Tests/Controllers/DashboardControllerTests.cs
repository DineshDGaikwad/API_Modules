using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using APIPropertyRegistry.Controllers;
using APIPropertyRegistry.Services.Interfaces;
using APIPropertyRegistry.DTOs;
using System;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Tests.Controllers
{
    public class DashboardControllerTests
    {
        private readonly Mock<IDashboardService> _mockService;
        private readonly DashboardController _controller;

        public DashboardControllerTests()
        {
            _mockService = new Mock<IDashboardService>();
            _controller = new DashboardController(_mockService.Object);
        }

        [Fact]
        public async Task GetAdminDashboard_WithoutDateRange_ShouldReturnOk()
        {
            // Arrange
            var dashboard = new AdminDashboardSummaryDto
            {
                TotalUsers = 10,
                TotalAgents = 3,
                TotalProperties = 15,
                TotalTransactions = 5,
                TotalRevenue = 2500000,
                PendingVerifications = 2,
                MonthlyRevenue = 500000,
                TopAgents = new System.Collections.Generic.List<TopAgentDto>()
            };
            _mockService.Setup(s => s.GetAdminDashboardAsync(null, null)).ReturnsAsync(dashboard);

            // Act
            var result = await _controller.GetAdminDashboard();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDashboard = Assert.IsType<AdminDashboardSummaryDto>(okResult.Value);
            Assert.Equal(10, returnedDashboard.TotalUsers);
            Assert.Equal(15, returnedDashboard.TotalProperties);
            _mockService.Verify(s => s.GetAdminDashboardAsync(null, null), Times.Once);
        }

        [Fact]
        public async Task GetAdminDashboard_WithDateRange_ShouldReturnOkWithFilteredData()
        {
            // Arrange
            var fromDate = DateTime.UtcNow.AddDays(-30);
            var toDate = DateTime.UtcNow;
            var dashboard = new AdminDashboardSummaryDto
            {
                TotalUsers = 10,
                TotalAgents = 3,
                TotalProperties = 15,
                TotalTransactions = 2,
                TotalRevenue = 1000000,
                PendingVerifications = 1,
                MonthlyRevenue = 1000000,
                TopAgents = new System.Collections.Generic.List<TopAgentDto>()
            };
            _mockService.Setup(s => s.GetAdminDashboardAsync(fromDate, toDate)).ReturnsAsync(dashboard);

            // Act
            var result = await _controller.GetAdminDashboard(fromDate, toDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDashboard = Assert.IsType<AdminDashboardSummaryDto>(okResult.Value);
            Assert.Equal(1000000, returnedDashboard.MonthlyRevenue);
            _mockService.Verify(s => s.GetAdminDashboardAsync(fromDate, toDate), Times.Once);
        }

        [Fact]
        public async Task GetAdminDashboard_ShouldIncludeTopAgents()
        {
            // Arrange
            var topAgents = new System.Collections.Generic.List<TopAgentDto>
            {
                new TopAgentDto { AgentId = 1, AgentName = "Agent 1", SalesCount = 5, TotalCommission = 50000 },
                new TopAgentDto { AgentId = 2, AgentName = "Agent 2", SalesCount = 3, TotalCommission = 30000 }
            };
            var dashboard = new AdminDashboardSummaryDto
            {
                TotalUsers = 10,
                TotalAgents = 3,
                TotalProperties = 15,
                TotalTransactions = 5,
                TotalRevenue = 2500000,
                PendingVerifications = 2,
                MonthlyRevenue = 500000,
                TopAgents = topAgents
            };
            _mockService.Setup(s => s.GetAdminDashboardAsync(null, null)).ReturnsAsync(dashboard);

            // Act
            var result = await _controller.GetAdminDashboard();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDashboard = Assert.IsType<AdminDashboardSummaryDto>(okResult.Value);
            Assert.Equal(2, returnedDashboard.TopAgents.Count);
            _mockService.Verify(s => s.GetAdminDashboardAsync(null, null), Times.Once);
        }

        [Fact]
        public async Task GetAgentDashboard_WithValidAgentId_ShouldReturnOk()
        {
            // Arrange
            var agentId = 1;
            var dashboard = new AgentDashboardDto
            {
                AgentId = agentId,
                AssignedProperties = 5,
                ActiveProperties = 3,
                VerifiedDocuments = 10,
                TotalSales = 2,
                TotalCommission = 50000
            };
            _mockService.Setup(s => s.GetAgentDashboardAsync(agentId, null, null)).ReturnsAsync(dashboard);

            // Act
            var result = await _controller.GetAgentDashboard(agentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDashboard = Assert.IsType<AgentDashboardDto>(okResult.Value);
            Assert.Equal(agentId, returnedDashboard.AgentId);
            Assert.Equal(5, returnedDashboard.AssignedProperties);
            _mockService.Verify(s => s.GetAgentDashboardAsync(agentId, null, null), Times.Once);
        }

        [Fact]
        public async Task GetAgentDashboard_WithDateRange_ShouldReturnOkWithFilteredData()
        {
            // Arrange
            var agentId = 1;
            var fromDate = DateTime.UtcNow.AddDays(-30);
            var toDate = DateTime.UtcNow;
            var dashboard = new AgentDashboardDto
            {
                AgentId = agentId,
                AssignedProperties = 5,
                ActiveProperties = 3,
                VerifiedDocuments = 10,
                TotalSales = 1,
                TotalCommission = 25000
            };
            _mockService.Setup(s => s.GetAgentDashboardAsync(agentId, fromDate, toDate)).ReturnsAsync(dashboard);

            // Act
            var result = await _controller.GetAgentDashboard(agentId, fromDate, toDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDashboard = Assert.IsType<AgentDashboardDto>(okResult.Value);
            Assert.Equal(1, returnedDashboard.TotalSales);
            _mockService.Verify(s => s.GetAgentDashboardAsync(agentId, fromDate, toDate), Times.Once);
        }

        [Fact]
        public async Task GetUserDashboard_WithValidUserId_ShouldReturnOk()
        {
            // Arrange
            var userId = 1;
            var dashboard = new UserDashboardDto
            {
                UserId = userId,
                OwnedProperties = 3,
                TotalTransactions = 2,
                TotalSpent = 1000000,
                UploadedDocuments = 5,
                PendingVerifications = 1
            };
            _mockService.Setup(s => s.GetUserDashboardAsync(userId, null, null)).ReturnsAsync(dashboard);

            // Act
            var result = await _controller.GetUserDashboard(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDashboard = Assert.IsType<UserDashboardDto>(okResult.Value);
            Assert.Equal(userId, returnedDashboard.UserId);
            Assert.Equal(3, returnedDashboard.OwnedProperties);
            _mockService.Verify(s => s.GetUserDashboardAsync(userId, null, null), Times.Once);
        }

        [Fact]
        public async Task GetUserDashboard_WithDateRange_ShouldReturnOkWithFilteredData()
        {
            // Arrange
            var userId = 1;
            var fromDate = DateTime.UtcNow.AddDays(-30);
            var toDate = DateTime.UtcNow;
            var dashboard = new UserDashboardDto
            {
                UserId = userId,
                OwnedProperties = 3,
                TotalTransactions = 1,
                TotalSpent = 500000,
                UploadedDocuments = 5,
                PendingVerifications = 0
            };
            _mockService.Setup(s => s.GetUserDashboardAsync(userId, fromDate, toDate)).ReturnsAsync(dashboard);

            // Act
            var result = await _controller.GetUserDashboard(userId, fromDate, toDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDashboard = Assert.IsType<UserDashboardDto>(okResult.Value);
            Assert.Equal(500000, returnedDashboard.TotalSpent);
            _mockService.Verify(s => s.GetUserDashboardAsync(userId, fromDate, toDate), Times.Once);
        }

        [Fact]
        public async Task GetUserDashboard_WithNoTransactions_ShouldReturnZeroValues()
        {
            // Arrange
            var userId = 999;
            var dashboard = new UserDashboardDto
            {
                UserId = userId,
                OwnedProperties = 0,
                TotalTransactions = 0,
                TotalSpent = 0,
                UploadedDocuments = 0,
                PendingVerifications = 0
            };
            _mockService.Setup(s => s.GetUserDashboardAsync(userId, null, null)).ReturnsAsync(dashboard);

            // Act
            var result = await _controller.GetUserDashboard(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDashboard = Assert.IsType<UserDashboardDto>(okResult.Value);
            Assert.Equal(0, returnedDashboard.OwnedProperties);
            Assert.Equal(0, returnedDashboard.TotalTransactions);
            _mockService.Verify(s => s.GetUserDashboardAsync(userId, null, null), Times.Once);
        }
    }
}