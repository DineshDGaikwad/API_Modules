using Xunit;
using Moq;
using System.Threading.Tasks;
using APIPropertyRegistry.Services.Implementations;
using APIPropertyRegistry.Repositories.Interfaces;
using APIPropertyRegistry.Services.Interfaces;
using APIPropertyRegistry.Models;
using APIPropertyRegistry.DTOs;
using System.Collections.Generic;

namespace APIPropertyRegistry.Tests.Services
{
    public class PropertyTransactionServiceTests
    {
        private readonly Mock<IPropertyTransactionRepository> _transactionRepo = new();
        private readonly Mock<IPropertyOwnershipService> _ownershipService = new();
        private readonly Mock<IAgentPropertyRepository> _agentPropertyRepo = new();
        private readonly Mock<IPropertyRepository> _propertyRepo = new();
        private readonly PropertyTransactionService _service;

        public PropertyTransactionServiceTests()
        {
            _service = new PropertyTransactionService(
                _transactionRepo.Object,
                _ownershipService.Object,
                _agentPropertyRepo.Object,
                _propertyRepo.Object);
        }

        [Fact]
        public async Task CreateTransactionAsync_ShouldAssignActiveAgentWhenNotProvided()
        {
            var dto = new PropertyTransactionCreateDto
            {
                PropertyId = 1,
                SellerId = 2,
                BuyerId = 3,
                TransactionAmount = 500000
            };

            var property = new Property
            {
                PropertyId = 1,
                IsAvailable = true,
                AgentId = 55
            };

            var persisted = new PropertyTransaction
            {
                TransactionId = 10,
                PropertyId = 1,
                SellerId = 2,
                BuyerId = 3,
                AgentId = 99,
                Amount = 500000,
                Status = "Pending",
                Stage = "AgentReview",
                Property = new Property { PropertyNumber = "PROP-1", Title = "Skyline", Address = "123 Market", City = "Metro", Price = 500000 },
                Buyer = new User { FullName = "Buyer" },
                Seller = new User { FullName = "Seller" },
                Agent = new User { FullName = "Agent" }
            };

            _propertyRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(property);
            _transactionRepo.Setup(r => r.HasActiveTransactionAsync(1)).ReturnsAsync(false);
            _agentPropertyRepo.Setup(r => r.GetActiveAgentIdByPropertyAsync(1)).ReturnsAsync(99);
            _transactionRepo.Setup(r => r.CreateAsync(It.IsAny<PropertyTransaction>())).ReturnsAsync((PropertyTransaction t) => { t.TransactionId = 10; return t; });
            _transactionRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(persisted);

            var result = await _service.CreateTransactionAsync(dto);

            Assert.Equal(99, result.AgentId);
            Assert.Equal("Pending", result.Status);
            Assert.Equal("AgentReview", result.Stage);
            _transactionRepo.Verify(r => r.CreateAsync(It.Is<PropertyTransaction>(t => t.AgentId == 99)), Times.Once);
        }

        [Fact]
        public async Task SubmitAgentDecisionAsync_Approve_ShouldMoveToAdminReview()
        {
            var transaction = new PropertyTransaction
            {
                TransactionId = 7,
                AgentId = 4,
                Status = "Pending",
                Stage = "AgentReview"
            };

            _transactionRepo.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(transaction);
            _transactionRepo.Setup(r => r.UpdateAsync(transaction)).Returns(Task.CompletedTask);

            var result = await _service.SubmitAgentDecisionAsync(new AgentTransactionDecisionDto
            {
                TransactionId = 7,
                AgentId = 4,
                Approve = true
            });

            Assert.True(result);
            Assert.Equal("Pending Admin", transaction.Status);
            Assert.Equal("AdminReview", transaction.Stage);
        }

        [Fact]
        public async Task SubmitAdminDecisionAsync_Approve_ShouldTransferOwnershipAndCloseOthers()
        {
            var transaction = new PropertyTransaction
            {
                TransactionId = 12,
                PropertyId = 3,
                BuyerId = 8,
                Status = "Pending Admin",
                Stage = "AdminReview"
            };

            var competing = new List<PropertyTransaction>
            {
                new PropertyTransaction
                {
                    TransactionId = 44,
                    PropertyId = 3,
                    Status = "Pending",
                    Stage = "AgentReview"
                }
            };

            var property = new Property
            {
                PropertyId = 3,
                IsAvailable = true
            };

            _transactionRepo.Setup(r => r.GetByIdAsync(12)).ReturnsAsync(transaction);
            _propertyRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(property);
            _propertyRepo.Setup(r => r.UpdateAsync(property)).Returns(Task.CompletedTask);
            _propertyRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
            _ownershipService.Setup(s => s.TransferOwnershipAsync(It.IsAny<PropertyOwnershipTransferDto>())).ReturnsAsync(true);
            _transactionRepo.Setup(r => r.GetPendingByPropertyAsync(3)).ReturnsAsync(competing);
            _transactionRepo.Setup(r => r.UpdateRangeAsync(It.IsAny<IEnumerable<PropertyTransaction>>())).Returns(Task.CompletedTask);
            _transactionRepo.Setup(r => r.UpdateAsync(transaction)).Returns(Task.CompletedTask);

            var result = await _service.SubmitAdminDecisionAsync(new AdminTransactionDecisionDto
            {
                TransactionId = 12,
                AdminId = 2,
                Approve = true
            });

            Assert.True(result);
            Assert.Equal("Approved", transaction.Status);
            Assert.Equal("Completed", transaction.Stage);
            _ownershipService.Verify(s => s.TransferOwnershipAsync(It.Is<PropertyOwnershipTransferDto>(d => d.PropertyId == 3 && d.NewOwnerId == 8)), Times.Once);
            _transactionRepo.Verify(r => r.UpdateRangeAsync(It.IsAny<IEnumerable<PropertyTransaction>>()), Times.Once);
        }

        [Fact]
        public async Task SubmitAdminDecisionAsync_Reject_ShouldArchiveTransaction()
        {
            var transaction = new PropertyTransaction
            {
                TransactionId = 30,
                PropertyId = 9,
                Status = "Pending Admin",
                Stage = "AdminReview"
            };

            _transactionRepo.Setup(r => r.GetByIdAsync(30)).ReturnsAsync(transaction);
            _transactionRepo.Setup(r => r.UpdateAsync(transaction)).Returns(Task.CompletedTask);

            var result = await _service.SubmitAdminDecisionAsync(new AdminTransactionDecisionDto
            {
                TransactionId = 30,
                AdminId = 5,
                Approve = false
            });

            Assert.True(result);
            Assert.Equal("Rejected by Admin", transaction.Status);
            Assert.True(transaction.IsArchived);
        }
    }
}
