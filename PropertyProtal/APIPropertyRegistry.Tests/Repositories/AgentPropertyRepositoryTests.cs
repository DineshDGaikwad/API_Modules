using Xunit;
using APIPropertyRegistry.Repositories.Implementations;
using APIPropertyRegistry.Models;
using System.Threading.Tasks;
using System.Linq;

namespace APIPropertyRegistry.Tests.Repositories
{
    public class AgentPropertyRepositoryTests
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllAssignments()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("AgentPropertyGetAll"))
            {
                var ap1 = TestFixtures.CreateTestAgentProperty(1, 1, 1);
                var ap2 = TestFixtures.CreateTestAgentProperty(2, 2, 2);
                context.AgentProperties.Add(ap1);
                context.AgentProperties.Add(ap2);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("AgentPropertyGetAll"))
            {
                var repository = new AgentPropertyRepository(context);
                var result = await repository.GetAllAsync();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnAssignment()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("AgentPropertyGetById"))
            {
                var ap = TestFixtures.CreateTestAgentProperty(1, 1, 1);
                context.AgentProperties.Add(ap);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("AgentPropertyGetById"))
            {
                var repository = new AgentPropertyRepository(context);
                var result = await repository.GetByIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.AgentPropertyId);
            }
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("AgentPropertyGetByIdInvalid"))
            {
                context.Database.EnsureCreated();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("AgentPropertyGetByIdInvalid"))
            {
                var repository = new AgentPropertyRepository(context);
                var result = await repository.GetByIdAsync(999);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetByAgentAsync_ShouldReturnAgentAssignments()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("AgentPropertyGetByAgent"))
            {
                var ap1 = TestFixtures.CreateTestAgentProperty(1, 1, 1);
                var ap2 = TestFixtures.CreateTestAgentProperty(2, 1, 2);
                var ap3 = TestFixtures.CreateTestAgentProperty(3, 2, 1);
                context.AgentProperties.Add(ap1);
                context.AgentProperties.Add(ap2);
                context.AgentProperties.Add(ap3);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("AgentPropertyGetByAgent"))
            {
                var repository = new AgentPropertyRepository(context);
                var result = await repository.GetByAgentAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
                Assert.All(result, ap => Assert.Equal(1, ap.AgentId));
            }
        }

        [Fact]
        public async Task AddAsync_ShouldAddNewAssignment()
        {
            // Arrange
            var ap = TestFixtures.CreateTestAgentProperty(1, 1, 1);

            using (var context = InMemoryDbContextFactory.CreateContext("AgentPropertyAdd"))
            {
                var repository = new AgentPropertyRepository(context);

                // Act
                var result = await repository.AddAsync(ap);
                await context.SaveChangesAsync();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.AgentPropertyId);
            }

            // Verify
            using (var context = InMemoryDbContextFactory.CreateContext("AgentPropertyAdd"))
            {
                var repository = new AgentPropertyRepository(context);
                var saved = await repository.GetByIdAsync(1);
                Assert.NotNull(saved);
            }
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateExistingAssignment()
        {
            // Arrange
            var ap = TestFixtures.CreateTestAgentProperty(1, 1, 1, false);
            using (var context = InMemoryDbContextFactory.CreateContext("AgentPropertyUpdate"))
            {
                context.AgentProperties.Add(ap);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("AgentPropertyUpdate"))
            {
                var repository = new AgentPropertyRepository(context);
                var assignment = await repository.GetByIdAsync(1);
                Assert.NotNull(assignment);
                assignment.IsApproved = true;
                assignment.Status = "Active";
                var result = await repository.UpdateAsync(assignment);

                // Assert
                Assert.True(result);
            }

            // Verify
            using (var context = InMemoryDbContextFactory.CreateContext("AgentPropertyUpdate"))
            {
                var repository = new AgentPropertyRepository(context);
                var updated = await repository.GetByIdAsync(1);
                Assert.NotNull(updated);
                Assert.True(updated.IsApproved);
                Assert.Equal("Active", updated.Status);
            }
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveAssignment()
        {
            // Arrange
            var ap = TestFixtures.CreateTestAgentProperty(1, 1, 1);
            using (var context = InMemoryDbContextFactory.CreateContext("AgentPropertyDelete"))
            {
                context.AgentProperties.Add(ap);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("AgentPropertyDelete"))
            {
                var repository = new AgentPropertyRepository(context);
                var result = await repository.DeleteAsync(1);

                // Assert
                Assert.True(result);
            }

            // Verify
            using (var context = InMemoryDbContextFactory.CreateContext("AgentPropertyDelete"))
            {
                var repository = new AgentPropertyRepository(context);
                var deleted = await repository.GetByIdAsync(1);
                Assert.Null(deleted);
            }
        }
    }
}