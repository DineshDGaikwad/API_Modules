using Xunit;
using APIPropertyRegistry.Repositories.Implementations;
using APIPropertyRegistry.Models;
using System.Threading.Tasks;
using System.Linq;

namespace APIPropertyRegistry.Tests.Repositories
{
    public class PropertyOwnershipRepositoryTests
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllOwnerships()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("OwnershipGetAll"))
            {
                var own1 = TestFixtures.CreateTestPropertyOwnership(1, 1, 1);
                var own2 = TestFixtures.CreateTestPropertyOwnership(2, 2, 2);
                context.PropertyOwnerships.Add(own1);
                context.PropertyOwnerships.Add(own2);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("OwnershipGetAll"))
            {
                var repository = new PropertyOwnershipRepository(context);
                var result = await repository.GetAllAsync();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnOwnership()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("OwnershipGetById"))
            {
                var own = TestFixtures.CreateTestPropertyOwnership(1, 1, 1);
                context.PropertyOwnerships.Add(own);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("OwnershipGetById"))
            {
                var repository = new PropertyOwnershipRepository(context);
                var result = await repository.GetByIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.OwnershipId);
            }
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("OwnershipGetByIdInvalid"))
            {
                context.Database.EnsureCreated();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("OwnershipGetByIdInvalid"))
            {
                var repository = new PropertyOwnershipRepository(context);
                var result = await repository.GetByIdAsync(999);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnUserOwnerships()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("OwnershipGetByUser"))
            {
                var own1 = TestFixtures.CreateTestPropertyOwnership(1, 1, 1);
                var own2 = TestFixtures.CreateTestPropertyOwnership(2, 2, 1);
                var own3 = TestFixtures.CreateTestPropertyOwnership(3, 3, 2);
                context.PropertyOwnerships.Add(own1);
                context.PropertyOwnerships.Add(own2);
                context.PropertyOwnerships.Add(own3);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("OwnershipGetByUser"))
            {
                var repository = new PropertyOwnershipRepository(context);
                var result = await repository.GetByUserIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
                Assert.All(result, o => Assert.Equal(1, o.UserId));
            }
        }

        [Fact]
        public async Task GetByPropertyIdAsync_ShouldReturnPropertyOwnerships()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("OwnershipGetByProperty"))
            {
                var own1 = TestFixtures.CreateTestPropertyOwnership(1, 1, 1);
                var own2 = TestFixtures.CreateTestPropertyOwnership(2, 1, 2);
                var own3 = TestFixtures.CreateTestPropertyOwnership(3, 2, 1);
                context.PropertyOwnerships.Add(own1);
                context.PropertyOwnerships.Add(own2);
                context.PropertyOwnerships.Add(own3);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("OwnershipGetByProperty"))
            {
                var repository = new PropertyOwnershipRepository(context);
                var result = await repository.GetByPropertyIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
                Assert.All(result, o => Assert.Equal(1, o.PropertyId));
            }
        }

        [Fact]
        public async Task AddAsync_ShouldAddNewOwnership()
        {
            // Arrange
            var own = TestFixtures.CreateTestPropertyOwnership(1, 1, 1);

            using (var context = InMemoryDbContextFactory.CreateContext("OwnershipAdd"))
            {
                var repository = new PropertyOwnershipRepository(context);

                // Act
                await repository.AddAsync(own);
                await repository.SaveChangesAsync();

                // Assert
                var result = await repository.GetByIdAsync(1);
                Assert.NotNull(result);
                Assert.Equal(1, result.OwnershipId);
            }
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateExistingOwnership()
        {
            // Arrange
            var own = TestFixtures.CreateTestPropertyOwnership(1, 1, 1, false);
            using (var context = InMemoryDbContextFactory.CreateContext("OwnershipUpdate"))
            {
                context.PropertyOwnerships.Add(own);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("OwnershipUpdate"))
            {
                var repository = new PropertyOwnershipRepository(context);
                var ownership = await repository.GetByIdAsync(1);
                Assert.NotNull(ownership);
                ownership.Verified = true;
                ownership.VerifiedBy = 2;
                ownership.Status = "Approved";
                await repository.UpdateAsync(ownership);
                await repository.SaveChangesAsync();

                // Verify
                var updated = await repository.GetByIdAsync(1);
                Assert.NotNull(updated);
                Assert.True(updated.Verified);
                Assert.Equal(2, updated.VerifiedBy);
                Assert.Equal("Approved", updated.Status);
            }
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveOwnership()
        {
            // Arrange
            var own = TestFixtures.CreateTestPropertyOwnership(1, 1, 1);
            using (var context = InMemoryDbContextFactory.CreateContext("OwnershipDelete"))
            {
                context.PropertyOwnerships.Add(own);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("OwnershipDelete"))
            {
                var repository = new PropertyOwnershipRepository(context);
                await repository.DeleteAsync(1);
                await repository.SaveChangesAsync();

                // Assert
                var deleted = await repository.GetByIdAsync(1);
                Assert.Null(deleted);
            }
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldReturnTrue()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("OwnershipSave"))
            {
                var repository = new PropertyOwnershipRepository(context);
                var own = TestFixtures.CreateTestPropertyOwnership(1, 1, 1);
                await repository.AddAsync(own);

                // Act
                var result = await repository.SaveChangesAsync();

                // Assert
                Assert.True(result);
            }
        }
    }
}