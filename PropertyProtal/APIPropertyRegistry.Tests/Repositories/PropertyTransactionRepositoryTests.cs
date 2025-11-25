using Xunit;
using APIPropertyRegistry.Repositories.Implementations;
using APIPropertyRegistry.Models;
using System.Threading.Tasks;
using System.Linq;

namespace APIPropertyRegistry.Tests.Repositories
{
    public class PropertyTransactionRepositoryTests
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTransactions()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("TransactionGetAll"))
            {
                var txn1 = TestFixtures.CreateTestPropertyTransaction(1, 1, 1, 2);
                var txn2 = TestFixtures.CreateTestPropertyTransaction(2, 2, 1, 3);
                context.PropertyTransactions.Add(txn1);
                context.PropertyTransactions.Add(txn2);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("TransactionGetAll"))
            {
                var repository = new PropertyTransactionRepository(context);
                var result = await repository.GetAllAsync();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnTransaction()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("TransactionGetById"))
            {
                var txn = TestFixtures.CreateTestPropertyTransaction(1, 1, 1, 2);
                context.PropertyTransactions.Add(txn);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("TransactionGetById"))
            {
                var repository = new PropertyTransactionRepository(context);
                var result = await repository.GetByIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.TransactionId);
            }
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("TransactionGetByIdInvalid"))
            {
                context.Database.EnsureCreated();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("TransactionGetByIdInvalid"))
            {
                var repository = new PropertyTransactionRepository(context);
                var result = await repository.GetByIdAsync(999);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetByBuyerAsync_ShouldReturnBuyerTransactions()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("TransactionGetByBuyer"))
            {
                var txn1 = TestFixtures.CreateTestPropertyTransaction(1, 1, 1, 2);
                var txn2 = TestFixtures.CreateTestPropertyTransaction(2, 2, 3, 2);
                var txn3 = TestFixtures.CreateTestPropertyTransaction(3, 3, 1, 4);
                context.PropertyTransactions.Add(txn1);
                context.PropertyTransactions.Add(txn2);
                context.PropertyTransactions.Add(txn3);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("TransactionGetByBuyer"))
            {
                var repository = new PropertyTransactionRepository(context);
                var result = await repository.GetByBuyerAsync(2);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
                Assert.All(result, t => Assert.Equal(2, t.BuyerId));
            }
        }

        [Fact]
        public async Task GetBySellerAsync_ShouldReturnSellerTransactions()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("TransactionGetBySeller"))
            {
                var txn1 = TestFixtures.CreateTestPropertyTransaction(1, 1, 1, 2);
                var txn2 = TestFixtures.CreateTestPropertyTransaction(2, 2, 1, 3);
                var txn3 = TestFixtures.CreateTestPropertyTransaction(3, 3, 4, 2);
                context.PropertyTransactions.Add(txn1);
                context.PropertyTransactions.Add(txn2);
                context.PropertyTransactions.Add(txn3);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("TransactionGetBySeller"))
            {
                var repository = new PropertyTransactionRepository(context);
                var result = await repository.GetBySellerAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
                Assert.All(result, t => Assert.Equal(1, t.SellerId));
            }
        }

        [Fact]
        public async Task CreateAsync_ShouldAddNewTransaction()
        {
            // Arrange
            var txn = TestFixtures.CreateTestPropertyTransaction(1, 1, 1, 2);

            using (var context = InMemoryDbContextFactory.CreateContext("TransactionCreate"))
            {
                var repository = new PropertyTransactionRepository(context);

                // Act
                var result = await repository.CreateAsync(txn);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.TransactionId);
                Assert.Equal("Pending", result.Status);
            }
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldUpdateTransactionStatus()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("TransactionUpdateStatus"))
            {
                var txn = TestFixtures.CreateTestPropertyTransaction(1, 1, 1, 2, null, "Pending");
                context.PropertyTransactions.Add(txn);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("TransactionUpdateStatus"))
            {
                var repository = new PropertyTransactionRepository(context);
                var result = await repository.UpdateStatusAsync(1, "Verified", 1);

                // Assert
                Assert.True(result);
            }

            // Verify
            using (var context = InMemoryDbContextFactory.CreateContext("TransactionUpdateStatus"))
            {
                var repository = new PropertyTransactionRepository(context);
                var updated = await repository.GetByIdAsync(1);
                Assert.NotNull(updated);
                Assert.Equal("Verified", updated.Status);
                Assert.Equal(1, updated.VerifiedBy);
            }
        }

        [Fact]
        public async Task UpdateStatusAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("TransactionUpdateStatusInvalid"))
            {
                context.Database.EnsureCreated();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("TransactionUpdateStatusInvalid"))
            {
                var repository = new PropertyTransactionRepository(context);
                var result = await repository.UpdateStatusAsync(999, "Verified", 1);

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnPendingStatus()
        {
            // Arrange
            var txn = TestFixtures.CreateTestPropertyTransaction(1, 1, 1, 2);

            using (var context = InMemoryDbContextFactory.CreateContext("TransactionCreatePending"))
            {
                var repository = new PropertyTransactionRepository(context);

                // Act
                var result = await repository.CreateAsync(txn);

                // Assert
                Assert.Equal("Pending", result.Status);
            }
        }

        [Fact]
        public async Task GetByBuyerAsync_ShouldReturnEmpty_WhenNoBuyerTransactions()
        {
            // Arrange
            using (var context = InMemoryDbContextFactory.CreateContext("TransactionGetByBuyerEmpty"))
            {
                context.Database.EnsureCreated();
            }

            // Act
            using (var context = InMemoryDbContextFactory.CreateContext("TransactionGetByBuyerEmpty"))
            {
                var repository = new PropertyTransactionRepository(context);
                var result = await repository.GetByBuyerAsync(999);

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result);
            }
        }
    }
}