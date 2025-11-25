using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using APIPropertyRegistry.Controllers;
using APIPropertyRegistry.Services.Interfaces;
using APIPropertyRegistry.Services.Implementations;
using Microsoft.Extensions.Options;
using APIPropertyRegistry.Helpers;
using APIPropertyRegistry.Models;
using APIPropertyRegistry.DTOs.UserDtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace APIPropertyRegistry.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly JwtService _jwtService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();

            var mockJwtOptions = new Mock<IOptions<JwtSettings>>();
            mockJwtOptions.Setup(o => o.Value).Returns(new JwtSettings
            {
                Key = "SuperStrongSecretKeyAtLeast32CharsLong!!",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpireMinutes = 60
            });

            _jwtService = new JwtService(mockJwtOptions.Object);
            _controller = new UserController(_mockUserService.Object, _jwtService);
        }

        // ===== REGISTER TESTS =====
        [Fact]
        public async Task Register_WithValidDto_ShouldReturnOk()
        {
            // Arrange
            var dto = TestFixtures.CreateUserCreateDto();
            _mockUserService.Setup(s => s.RegisterUserAsync(dto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockUserService.Verify(s => s.RegisterUserAsync(dto), Times.Once);
        }

        [Fact]
        public async Task Register_WhenRegistrationFails_ShouldReturnBadRequest()
        {
            // Arrange
            var dto = TestFixtures.CreateUserCreateDto();
            _mockUserService.Setup(s => s.RegisterUserAsync(dto)).ReturnsAsync(false);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult.Value);
            _mockUserService.Verify(s => s.RegisterUserAsync(dto), Times.Once);
        }

        // ===== LOGIN TESTS =====
        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnOkWithToken()
        {
            // Arrange
            var loginDto = TestFixtures.CreateUserLoginDto();
            var user = TestFixtures.CreateTestUser();

            _mockUserService.Setup(s => s.LoginAsync(loginDto)).ReturnsAsync(user);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            // Verify response type and check for token property using reflection
            var responseType = okResult.Value!.GetType();
            var tokenProperty = responseType.GetProperty("token");
            var userProperty = responseType.GetProperty("user");
            
            Assert.NotNull(tokenProperty);
            Assert.NotNull(userProperty);
            Assert.NotNull(tokenProperty.GetValue(okResult.Value));
            _mockUserService.Verify(s => s.LoginAsync(loginDto), Times.Once);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginDto = TestFixtures.CreateUserLoginDto();
            _mockUserService.Setup(s => s.LoginAsync(loginDto)).ReturnsAsync((User?)null);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.NotNull(unauthorizedResult.Value);
            _mockUserService.Verify(s => s.LoginAsync(loginDto), Times.Once);
        }

        // ===== GET ALL USERS TESTS =====
        [Fact]
        public async Task GetAll_ShouldReturnOkWithUsersList()
        {
            // Arrange
            var users = new List<UserResponseDto>
            {
                new UserResponseDto { UserId = 1, FullName = "User 1", Email = "user1@test.com", Role = "User", IsApproved = true, CreatedAt = System.DateTime.UtcNow },
                new UserResponseDto { UserId = 2, FullName = "User 2", Email = "user2@test.com", Role = "Agent", IsApproved = false, CreatedAt = System.DateTime.UtcNow }
            };

            _mockUserService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedUsers.Count());
            _mockUserService.Verify(s => s.GetAllUsersAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenNoUsersExist_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyList = new List<UserResponseDto>();
            _mockUserService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(emptyList);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserResponseDto>>(okResult.Value);
            Assert.Empty(returnedUsers);
            _mockUserService.Verify(s => s.GetAllUsersAsync(), Times.Once);
        }

        // ===== GET USER BY ID TESTS =====
        [Fact]
        public async Task GetById_WithValidId_ShouldReturnOkWithUser()
        {
            // Arrange
            var userId = 1;
            var userDto = new UserResponseDto
            {
                UserId = userId,
                FullName = "Test User",
                Email = "test@test.com",
                Role = "User",
                IsApproved = true,
                CreatedAt = System.DateTime.UtcNow
            };

            _mockUserService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(userDto);

            // Act
            var result = await _controller.GetById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<UserResponseDto>(okResult.Value);
            Assert.Equal(userId, returnedUser.UserId);
            _mockUserService.Verify(s => s.GetUserByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var userId = 999;
            _mockUserService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync((UserResponseDto?)null);

            // Act
            var result = await _controller.GetById(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
            _mockUserService.Verify(s => s.GetUserByIdAsync(userId), Times.Once);
        }

        // ===== GET USER BY EMAIL TESTS =====
        [Fact]
        public async Task GetByEmail_WithValidEmail_ShouldReturnOkWithUser()
        {
            // Arrange
            var email = "test@test.com";
            var userDto = new UserResponseDto
            {
                UserId = 1,
                FullName = "Test User",
                Email = email,
                Role = "User",
                IsApproved = true,
                CreatedAt = System.DateTime.UtcNow
            };

            _mockUserService.Setup(s => s.GetUserByEmailAsync(email)).ReturnsAsync(userDto);

            // Act
            var result = await _controller.GetByEmail(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<UserResponseDto>(okResult.Value);
            Assert.Equal(email, returnedUser.Email);
            _mockUserService.Verify(s => s.GetUserByEmailAsync(email), Times.Once);
        }

        [Fact]
        public async Task GetByEmail_WithInvalidEmail_ShouldReturnNotFound()
        {
            // Arrange
            var email = "nonexistent@test.com";
            _mockUserService.Setup(s => s.GetUserByEmailAsync(email)).ReturnsAsync((UserResponseDto?)null);

            // Act
            var result = await _controller.GetByEmail(email);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
            _mockUserService.Verify(s => s.GetUserByEmailAsync(email), Times.Once);
        }

        // ===== GET USERS BY ROLE TESTS =====
        [Fact]
        public async Task GetByRole_WithValidRole_ShouldReturnOkWithUsersList()
        {
            // Arrange
            var role = "Agent";
            var users = new List<UserResponseDto>
            {
                new UserResponseDto { UserId = 1, FullName = "Agent 1", Email = "agent1@test.com", Role = role, IsApproved = true, CreatedAt = System.DateTime.UtcNow },
                new UserResponseDto { UserId = 2, FullName = "Agent 2", Email = "agent2@test.com", Role = role, IsApproved = true, CreatedAt = System.DateTime.UtcNow }
            };

            _mockUserService.Setup(s => s.GetUsersByRoleAsync(role)).ReturnsAsync(users);

            // Act
            var result = await _controller.GetByRole(role);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedUsers.Count());
            _mockUserService.Verify(s => s.GetUsersByRoleAsync(role), Times.Once);
        }

        // ===== UPDATE USER TESTS =====
        [Fact]
        public async Task Update_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var userId = 1;
            var updateDto = new UserUpdateDto { FullName = "Updated Name", Email = "updated@test.com" };
            _mockUserService.Setup(s => s.UpdateUserAsync(userId, updateDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Update(userId, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockUserService.Verify(s => s.UpdateUserAsync(userId, updateDto), Times.Once);
        }

        [Fact]
        public async Task Update_WithInvalidUserId_ShouldReturnNotFound()
        {
            // Arrange
            var userId = 999;
            var updateDto = new UserUpdateDto { FullName = "Updated Name" };
            _mockUserService.Setup(s => s.UpdateUserAsync(userId, updateDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.Update(userId, updateDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
            _mockUserService.Verify(s => s.UpdateUserAsync(userId, updateDto), Times.Once);
        }

        // ===== DELETE USER TESTS =====
        [Fact]
        public async Task Delete_WithValidId_ShouldReturnOk()
        {
            // Arrange
            var userId = 1;
            _mockUserService.Setup(s => s.DeleteUserAsync(userId)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockUserService.Verify(s => s.DeleteUserAsync(userId), Times.Once);
        }

        [Fact]
        public async Task Delete_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var userId = 999;
            _mockUserService.Setup(s => s.DeleteUserAsync(userId)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
            _mockUserService.Verify(s => s.DeleteUserAsync(userId), Times.Once);
        }

        // ===== PENDING AGENTS TESTS =====
        [Fact]
        public async Task GetPendingAgents_ShouldReturnOkWithPendingAgentsList()
        {
            // Arrange
            var pendingAgents = new List<UserResponseDto>
            {
                new UserResponseDto { UserId = 1, FullName = "Pending Agent", Email = "agent@test.com", Role = "Agent", IsApproved = false, CreatedAt = System.DateTime.UtcNow }
            };

            _mockUserService.Setup(s => s.GetPendingAgentsAsync()).ReturnsAsync(pendingAgents);

            // Act
            var result = await _controller.GetPendingAgents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAgents = Assert.IsAssignableFrom<IEnumerable<UserResponseDto>>(okResult.Value);
            Assert.Single(returnedAgents);
            _mockUserService.Verify(s => s.GetPendingAgentsAsync(), Times.Once);
        }

        // ===== APPROVE AGENT TESTS =====
        [Fact]
        public async Task ApproveAgent_WithApproval_ShouldReturnOk()
        {
            // Arrange
            var dto = new AgentApprovalDto { AgentId = 1, Approve = true };
            var adminId = 1;
            _mockUserService.Setup(s => s.ApproveAgentAsync(dto.AgentId, dto.Approve, adminId)).ReturnsAsync(true);

            // Act
            var result = await _controller.ApproveAgent(dto, adminId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockUserService.Verify(s => s.ApproveAgentAsync(dto.AgentId, dto.Approve, adminId), Times.Once);
        }

        [Fact]
        public async Task ApproveAgent_WithRejection_ShouldReturnOk()
        {
            // Arrange
            var dto = new AgentApprovalDto { AgentId = 1, Approve = false };
            var adminId = 1;
            _mockUserService.Setup(s => s.ApproveAgentAsync(dto.AgentId, dto.Approve, adminId)).ReturnsAsync(true);

            // Act
            var result = await _controller.ApproveAgent(dto, adminId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockUserService.Verify(s => s.ApproveAgentAsync(dto.AgentId, dto.Approve, adminId), Times.Once);
        }

        [Fact]
        public async Task ApproveAgent_WithInvalidAgentId_ShouldReturnNotFound()
        {
            // Arrange
            var dto = new AgentApprovalDto { AgentId = 999, Approve = true };
            var adminId = 1;
            _mockUserService.Setup(s => s.ApproveAgentAsync(dto.AgentId, dto.Approve, adminId)).ReturnsAsync(false);

            // Act
            var result = await _controller.ApproveAgent(dto, adminId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
            _mockUserService.Verify(s => s.ApproveAgentAsync(dto.AgentId, dto.Approve, adminId), Times.Once);
        }
    }
}