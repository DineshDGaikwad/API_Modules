using Xunit;
using APIPropertyRegistry.Repositories.Implementations;
using APIPropertyRegistry.Models;
using System.Threading.Tasks;
using System.Linq;
using APIPropertyRegistry.Tests.Helpers;

namespace APIPropertyRegistry.Tests.Repositories
{
    public class PropertyRepositoryTests
    {
        // ===== GET BY ID TESTS =====
        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnProperty()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var property = TestFixtures.CreateTestProperty(1);
            context.Properties.Add(property);
            await context.SaveChangesAsync();

            var repository = new PropertyRepository(context);

            // Act
            var result = await repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.PropertyId);
            Assert.Equal("Test Property 1", result.Title);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var repository = new PropertyRepository(context);

            // Act
            var result = await repository.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        // ===== GET ALL TESTS =====
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProperties()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var properties = TestFixtures.CreateTestPropertyList(3);
            context.Properties.AddRange(properties);
            await context.SaveChangesAsync();

            var repository = new PropertyRepository(context);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_WhenNoProperties_ShouldReturnEmptyList()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var repository = new PropertyRepository(context);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        // ===== GET BY OWNER ID TESTS =====
        [Fact]
        public async Task GetByOwnerIdAsync_WithValidOwnerId_ShouldReturnPropertiesByOwner()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            
            // First add owners to context
            var owner1 = TestFixtures.CreateTestUser(1, "owner1@test.com", "User");
            var owner2 = TestFixtures.CreateTestUser(2, "owner2@test.com", "User");
            context.Users.Add(owner1);
            context.Users.Add(owner2);
            await context.SaveChangesAsync();

            var properties = new[]
            {
                TestFixtures.CreateTestProperty(1, 1, 1),
                TestFixtures.CreateTestProperty(2, 1, 1),
                TestFixtures.CreateTestProperty(3, 2, 1)
            };
            context.Properties.AddRange(properties);
            await context.SaveChangesAsync();

            var repository = new PropertyRepository(context);

            // Act
            var result = await repository.GetByOwnerIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.Equal(1, p.OwnerId));
        }

        // ===== GET PENDING TESTS =====
        [Fact]
        public async Task GetPendingAsync_ShouldReturnOnlyPendingProperties()
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

            var repository = new PropertyRepository(context);

            // Act
            var result = await repository.GetPendingAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.False(p.IsApproved));
        }

        // ===== ADD TESTS =====
        [Fact]
        public async Task AddAsync_ShouldAddPropertyToContext()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var repository = new PropertyRepository(context);
            var property = TestFixtures.CreateTestProperty(1);

            // Act
            await repository.AddAsync(property);
            await context.SaveChangesAsync();

            // Assert
            var savedProperty = await context.Properties.FindAsync(1);
            Assert.NotNull(savedProperty);
            Assert.Equal("Test Property 1", savedProperty.Title);
        }

        // ===== UPDATE TESTS =====
        [Fact]
        public async Task UpdateAsync_ShouldUpdateProperty()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var property = TestFixtures.CreateTestProperty(1);
            context.Properties.Add(property);
            await context.SaveChangesAsync();

            var repository = new PropertyRepository(context);
            property.Title = "Updated Property";
            property.Price = 600000;

            // Act
            await repository.UpdateAsync(property);
            await context.SaveChangesAsync();

            // Assert
            var updatedProperty = await context.Properties.FindAsync(1);
            Assert.NotNull(updatedProperty);
            Assert.Equal("Updated Property", updatedProperty.Title);
            Assert.Equal(600000, updatedProperty.Price);
        }

        // ===== DELETE TESTS =====
        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldRemoveProperty()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var property = TestFixtures.CreateTestProperty(1);
            context.Properties.Add(property);
            await context.SaveChangesAsync();

            var repository = new PropertyRepository(context);

            // Act
            await repository.DeleteAsync(1);
            await context.SaveChangesAsync();

            // Assert
            var deletedProperty = await context.Properties.FindAsync(1);
            Assert.Null(deletedProperty);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldNotThrow()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var repository = new PropertyRepository(context);

            // Act & Assert (should not throw)
            await repository.DeleteAsync(999);
            await context.SaveChangesAsync();
        }

        // ===== SAVE CHANGES TESTS =====
        [Fact]
        public async Task SaveChangesAsync_ShouldReturnTrueWhenChangesSaved()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var property = TestFixtures.CreateTestProperty(1);
            context.Properties.Add(property);

            var repository = new PropertyRepository(context);

            // Act
            var result = await repository.SaveChangesAsync();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldReturnFalseWhenNoChanges()
        {
            // Arrange
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var repository = new PropertyRepository(context);

            // Act
            var result = await repository.SaveChangesAsync();

            // Assert
            Assert.False(result);
        }
    }
}