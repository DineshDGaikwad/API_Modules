using Xunit;
using Moq;
using APIPropertyRegistry.Services.Implementations;
using APIPropertyRegistry.Repositories.Interfaces;
using APIPropertyRegistry.Models;
using APIPropertyRegistry.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace APIPropertyRegistry.Tests.Services
{
    public class AgentPropertyServiceTests
    {
        private readonly Mock<IAgentPropertyRepository> _mockRepository;
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly AgentPropertyService _service;

        public AgentPropertyServiceTests()
        {
            _mockRepository = new Mock<IAgentPropertyRepository>();
            _mockContext = new Mock<ApplicationDbContext>();
            _service = new AgentPropertyService(_mockRepository.Object, _mockContext.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllAssignments()
        {
            // Arrange
            var assignments = new List<AgentProperty>
            {
                TestFixtures.CreateTestAgentProperty(1, 1, 1),
                TestFixtures.CreateTestAgentProperty(2, 2, 2)
            };
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(assignments);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByAgentAsync_ShouldReturnAgentAssignments()
        {
            // Arrange
            var agentId = 1;
            var assignments = new List<AgentProperty>
            {
                TestFixtures.CreateTestAgentProperty(1, agentId, 1),
                TestFixtures.CreateTestAgentProperty(2, agentId, 2)
            };
            _mockRepository.Setup(r => r.GetByAgentAsync(agentId)).ReturnsAsync(assignments);

            // Act
            var result = await _service.GetByAgentAsync(agentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepository.Verify(r => r.GetByAgentAsync(agentId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnAssignment()
        {
            // Arrange
            var assignment = TestFixtures.CreateTestAgentProperty(1, 1, 1);
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(assignment);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.AgentPropertyId);
            _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((AgentProperty?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldReturnCreatedAssignment()
        {
            // Arrange
            var createDto = TestFixtures.CreateAgentPropertyCreateDto(1, 1);
            var agent = TestFixtures.CreateTestUser(1, "agent@test.com", "agent");
            var property = TestFixtures.CreateTestProperty(1, 1, 1, true);

            var users = new[] { agent }.AsQueryable();
            var properties = new[] { property }.AsQueryable();

            var mockUsersDbSet = MockDbSet(users);
            var mockPropsDbSet = MockDbSet(properties);

            _mockContext.Setup(c => c.Users).Returns(mockUsersDbSet.Object);
            _mockContext.Setup(c => c.Properties).Returns(mockPropsDbSet.Object);

            var createdAssignment = TestFixtures.CreateTestAgentProperty(1, 1, 1);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<AgentProperty>())).ReturnsAsync(createdAssignment);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.AgentPropertyId);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<AgentProperty>()), Times.Once);
        }

        [Fact]
        public async Task ApproveAsync_WithValidId_ShouldUpdateApprovalStatus()
        {
            // Arrange
            var approveDto = TestFixtures.CreateAgentPropertyApproveDto(1, true);
            var assignment = TestFixtures.CreateTestAgentProperty(1, 1, 1, false);
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(assignment);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<AgentProperty>())).ReturnsAsync(true);

            // Act
            var result = await _service.ApproveAsync(approveDto);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<AgentProperty>()), Times.Once);
        }

        [Fact]
        public async Task ApproveAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var approveDto = TestFixtures.CreateAgentPropertyApproveDto(999, true);
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((AgentProperty?)null);

            // Act
            var result = await _service.ApproveAsync(approveDto);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _service.DeleteAsync(999);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.DeleteAsync(999), Times.Once);
        }

        private Mock<IQueryable<T>> MockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<IQueryable<T>>();
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(default))
                .Returns(new AsyncEnumerator<T>(data.GetEnumerator()));
            mockSet.Setup(m => m.Provider).Returns(new AsyncQueryProvider<T>(data.Provider));
            mockSet.Setup(m => m.Expression).Returns(data.Expression);
            mockSet.Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet;
        }
    }

    public class AsyncEnumerator<T> : System.Collections.Generic.IAsyncEnumerator<T>
    {
        private readonly System.Collections.Generic.IEnumerator<T> _enumerator;
        public T Current => _enumerator.Current;

        public AsyncEnumerator(System.Collections.Generic.IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        public System.Threading.Tasks.ValueTask DisposeAsync()
        {
            _enumerator?.Dispose();
            return default;
        }

        public System.Threading.Tasks.ValueTask<bool> MoveNextAsync()
        {
            return new System.Threading.Tasks.ValueTask<bool>(_enumerator.MoveNext());
        }
    }

    public class AsyncQueryProvider<T> : System.Linq.Expressions.ExpressionVisitor, IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        public AsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
        {
            return new AsyncEnumerable<T>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
        {
            return new AsyncEnumerable<TElement>(expression);
        }

        public object Execute(System.Linq.Expressions.Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public System.Threading.Tasks.ValueTask<object> ExecuteAsync(System.Linq.Expressions.Expression expression,
            System.Threading.CancellationToken cancellationToken = default)
        {
            return new System.Threading.Tasks.ValueTask<object>(_inner.Execute(expression));
        }
    }

    public class AsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public AsyncEnumerable(System.Linq.Expressions.Expression expression)
            : base(expression)
        {
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(System.Threading.CancellationToken cancellationToken = default)
        {
            return new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new AsyncQueryProvider<T>(this);
    }

    public interface IAsyncQueryProvider : IQueryProvider
    {
        IAsyncEnumerable<TElement> ExecuteAsync<TElement>(System.Linq.Expressions.Expression expression);
    }
}