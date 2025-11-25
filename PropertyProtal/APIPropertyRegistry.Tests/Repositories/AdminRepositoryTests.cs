using Xunit;
using APIPropertyRegistry.Repositories.Implementations;
using APIPropertyRegistry.Models;
using System.Threading.Tasks;
using System.Linq;
using APIPropertyRegistry.Tests.Helpers;

namespace APIPropertyRegistry.Tests.Repositories
{
    public class AdminRepositoryTests
    {
        // ===== GET PENDING AGENTS TESTS =====
        [Fact]
        public async Task GetPendingAgentsAsync_ShouldReturnOnlyPendingAgents()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var users = new[]
            {
                TestFixtures.CreateTestUser(1, "agent1@test.com", "agent"),
                TestFixtures.CreateTestUser(2, "agent2@test.com", "agent"),
                TestFixtures.CreateTestUser(3, "agent3@test.com", "agent")
            };
            users[0].IsApproved = false;
            users[1].IsApproved = false;
            users[2].IsApproved = true;

            context.Users.AddRange(users);
            await context.SaveChangesAsync();

            var repository = new AdminRepository(context);

            // Act
            var result = await repository.GetPendingAgentsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, a => Assert.False(a.IsApproved));
        }

        [Fact]
        public async Task GetPendingAgentsAsync_WhenNoPendingAgents_ShouldReturnEmptyList()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var users = TestFixtures.CreateTestUserList(2);
            users.ForEach(u => u.IsApproved = true);
            context.Users.AddRange(users);
            await context.SaveChangesAsync();

            var repository = new AdminRepository(context);

            // Act
            var result = await repository.GetPendingAgentsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        // ===== GET APPROVED AGENTS TESTS =====
        [Fact]
        public async Task GetApprovedAgentsAsync_ShouldReturnOnlyApprovedAgents()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var users = new[]
            {
                TestFixtures.CreateTestUser(1, "agent1@test.com", "agent"),
                TestFixtures.CreateTestUser(2, "agent2@test.com", "agent"),
                TestFixtures.CreateTestUser(3, "agent3@test.com", "agent")
            };
            users[0].IsApproved = true;
            users[1].IsApproved = true;
            users[2].IsApproved = false;

            context.Users.AddRange(users);
            await context.SaveChangesAsync();

            var repository = new AdminRepository(context);

            // Act
            var result = await repository.GetApprovedAgentsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, a => Assert.True(a.IsApproved));
        }

        // ===== GET AGENT BY ID TESTS =====
        [Fact]
        public async Task GetAgentByIdAsync_WithValidId_ShouldReturnAgent()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var user = TestFixtures.CreateTestUser(1, "agent@test.com", "agent");
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var repository = new AdminRepository(context);

            // Act
            var result = await repository.GetAgentByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
            Assert.Equal("agent@test.com", result.Email);
        }

        [Fact]
        public async Task GetAgentByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var repository = new AdminRepository(context);

            // Act
            var result = await repository.GetAgentByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        // ===== UPDATE AGENT TESTS =====
        [Fact]
        public async Task UpdateAgentAsync_ShouldUpdateAgent()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var user = TestFixtures.CreateTestUser(1, "agent@test.com", "agent");
            user.IsApproved = false;
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var repository = new AdminRepository(context);
            user.IsApproved = true;
            user.Remarks = "Approved by admin";

            // Act
            await repository.UpdateAgentAsync(user);

            // Assert
            var updatedUser = await context.Users.FindAsync(1);
            Assert.NotNull(updatedUser);
            Assert.True(updatedUser.IsApproved);
            Assert.Equal("Approved by admin", updatedUser.Remarks);
        }

        // ===== GET PENDING PROPERTIES TESTS =====
        [Fact]
        public async Task GetPendingPropertiesAsync_ShouldReturnOnlyPendingProperties()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var properties = new[]
            {
                TestFixtures.CreateTestProperty(1, 1, 1, false),
                TestFixtures.CreateTestProperty(2, 2, 1, false),
                TestFixtures.CreateTestProperty(3, 3, 1, true)
            };
            context.Properties.AddRange(properties);
            await context.SaveChangesAsync();

            var repository = new AdminRepository(context);

            // Act
            var result = await repository.GetPendingPropertiesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.False(p.IsApproved));
        }

        [Fact]
        public async Task GetPendingPropertiesAsync_WhenNoProperties_ShouldReturnEmptyList()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var properties = new[]
            {
                TestFixtures.CreateTestProperty(1, 1, 1, true),
                TestFixtures.CreateTestProperty(2, 2, 1, true)
            };
            context.Properties.AddRange(properties);
            await context.SaveChangesAsync();

            var repository = new AdminRepository(context);

            // Act
            var result = await repository.GetPendingPropertiesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        // ===== GET APPROVED PROPERTIES TESTS =====
        [Fact]
        public async Task GetApprovedPropertiesAsync_ShouldReturnOnlyApprovedProperties()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var properties = new[]
            {
                TestFixtures.CreateTestProperty(1, 1, 1, true),
                TestFixtures.CreateTestProperty(2, 2, 1, true),
                TestFixtures.CreateTestProperty(3, 3, 1, false)
            };
            context.Properties.AddRange(properties);
            await context.SaveChangesAsync();

            var repository = new AdminRepository(context);

            // Act
            var result = await repository.GetApprovedPropertiesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.True(p.IsApproved));
        }

        // ===== GET PROPERTY BY ID TESTS =====
        [Fact]
        public async Task GetPropertyByIdAsync_WithValidId_ShouldReturnProperty()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var property = TestFixtures.CreateTestProperty(1);
            context.Properties.Add(property);
            await context.SaveChangesAsync();

            var repository = new AdminRepository(context);

            // Act
            var result = await repository.GetPropertyByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.PropertyId);
            Assert.Equal("Test Property 1", result.Title);
        }

        [Fact]
        public async Task GetPropertyByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var repository = new AdminRepository(context);

            // Act
            var result = await repository.GetPropertyByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        // ===== UPDATE PROPERTY TESTS =====
        [Fact]
        public async Task UpdatePropertyAsync_ShouldUpdateProperty()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var property = TestFixtures.CreateTestProperty(1, 1, 1, false);
            context.Properties.Add(property);
            await context.SaveChangesAsync();

            var repository = new AdminRepository(context);
            property.IsApproved = true;
            property.Status = "Approved";
            property.ApprovedBy = 1;

            // Act
            await repository.UpdatePropertyAsync(property);
            await context.SaveChangesAsync();

            // Assert
            var updatedProperty = await context.Properties.FindAsync(1);
            Assert.NotNull(updatedProperty);
            Assert.True(updatedProperty.IsApproved);
            Assert.Equal("Approved", updatedProperty.Status);
            Assert.Equal(1, updatedProperty.ApprovedBy);
        }
    }
}