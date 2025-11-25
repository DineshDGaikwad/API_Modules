using Xunit;
using APIPropertyRegistry.Repositories.Implementations;
using APIPropertyRegistry.Models;
using System.Threading.Tasks;
using System.Linq;

namespace APIPropertyRegistry.Tests.Repositories
{
    public class DocumentRepositoryTests
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllDocuments()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("DocumentGetAll"))
            {
                var doc1 = TestFixtures.CreateTestDocument(1, 1, 1);
                var doc2 = TestFixtures.CreateTestDocument(2, 1, 1);
                context.Documents.Add(doc1);
                context.Documents.Add(doc2);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("DocumentGetAll"))
            {
                var repository = new DocumentRepository(context);
                var result = await repository.GetAllAsync();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
            }
        }

        [Fact]
        public async Task GetPendingAsync_ShouldReturnUnverifiedDocuments()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("DocumentGetPending"))
            {
                var verified = TestFixtures.CreateTestDocument(1, 1, 1, true);
                var pending = TestFixtures.CreateTestDocument(2, 1, 1, false);
                context.Documents.Add(verified);
                context.Documents.Add(pending);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("DocumentGetPending"))
            {
                var repository = new DocumentRepository(context);
                var result = await repository.GetPendingAsync();

                // Assert
                Assert.NotNull(result);
                Assert.Single(result);
                Assert.False(result.First().Verified);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDocument()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("DocumentGetById"))
            {
                var doc = TestFixtures.CreateTestDocument(1, 1, 1);
                context.Documents.Add(doc);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("DocumentGetById"))
            {
                var repository = new DocumentRepository(context);
                var result = await repository.GetByIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.DocumentId);
            }
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("DocumentGetByIdInvalid"))
            {
                context.Database.EnsureCreated();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("DocumentGetByIdInvalid"))
            {
                var repository = new DocumentRepository(context);
                var result = await repository.GetByIdAsync(999);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task AddAsync_ShouldAddNewDocument()
        {
            // Arrange
            var doc = TestFixtures.CreateTestDocument(1, 1, 1);

            using (var context = InMemoryDbContextFactory.CreateContext("DocumentAdd"))
            {
                var repository = new DocumentRepository(context);

                // Act
                var result = await repository.AddAsync(doc);
                await context.SaveChangesAsync();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.DocumentId);
            }

            // Verify
            using (var context = InMemoryDbContextFactory.CreateContext("DocumentAdd"))
            {
                var repository = new DocumentRepository(context);
                var saved = await repository.GetByIdAsync(1);
                Assert.NotNull(saved);
                Assert.False(saved.Verified);
            }
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateExistingDocument()
        {
            // Arrange
            var doc = TestFixtures.CreateTestDocument(1, 1, 1, false);
            using (var context = InMemoryDbContextFactory.CreateContext("DocumentUpdate"))
            {
                context.Documents.Add(doc);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("DocumentUpdate"))
            {
                var repository = new DocumentRepository(context);
                var document = await repository.GetByIdAsync(1);
                Assert.NotNull(document);
                document.Verified = true;
                document.VerifiedBy = 2;
                var result = await repository.UpdateAsync(document);

                // Assert
                Assert.True(result);
            }

            // Verify
            using (var context = InMemoryDbContextFactory.CreateContext("DocumentUpdate"))
            {
                var repository = new DocumentRepository(context);
                var updated = await repository.GetByIdAsync(1);
                Assert.NotNull(updated);
                Assert.True(updated.Verified);
                Assert.Equal(2, updated.VerifiedBy);
            }
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveDocument()
        {
            // Arrange
            var doc = TestFixtures.CreateTestDocument(1, 1, 1);
            using (var context = InMemoryDbContextFactory.CreateContext("DocumentDelete"))
            {
                context.Documents.Add(doc);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("DocumentDelete"))
            {
                var repository = new DocumentRepository(context);
                var result = await repository.DeleteAsync(1);

                // Assert
                Assert.True(result);
            }

            // Verify
            using (var context = InMemoryDbContextFactory.CreateContext("DocumentDelete"))
            {
                var repository = new DocumentRepository(context);
                var deleted = await repository.GetByIdAsync(1);
                Assert.Null(deleted);
            }
        }

        [Fact]
        public async Task GetPendingAsync_WithMultipleDocuments_ShouldOnlyReturnPending()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("DocumentPendingMultiple"))
            {
                var pending1 = TestFixtures.CreateTestDocument(1, 1, 1, false);
                var verified = TestFixtures.CreateTestDocument(2, 1, 1, true);
                var pending2 = TestFixtures.CreateTestDocument(3, 1, 1, false);
                context.Documents.Add(pending1);
                context.Documents.Add(verified);
                context.Documents.Add(pending2);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("DocumentPendingMultiple"))
            {
                var repository = new DocumentRepository(context);
                var result = await repository.GetPendingAsync();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
                Assert.All(result, d => Assert.False(d.Verified));
            }
        }
    }
}