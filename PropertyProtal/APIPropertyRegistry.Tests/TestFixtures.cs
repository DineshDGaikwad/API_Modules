using APIPropertyRegistry.Models;
using APIPropertyRegistry.DTOs.UserDtos;
using APIPropertyRegistry.DTOs.PropertyDtos;
using APIPropertyRegistry.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace APIPropertyRegistry.Tests
{
    public static class TestFixtures
    {
        // ===== USER FIXTURES =====
        public static User CreateTestUser(int id = 1, string email = "user@test.com", string role = "User")
        {
            return new User
            {
                UserId = id,
                FullName = $"Test User {id}",
                Email = email,
                Password = "TestPassword@123",
                Role = role,
                CreatedAt = DateTime.UtcNow,
                IsApproved = true
            };
        }

        public static UserCreateDto CreateUserCreateDto()
        {
            return new UserCreateDto
            {
                FullName = "New User",
                Email = "newuser@test.com",
                Password = "SecurePassword@123",
                Role = "User"
            };
        }

        public static UserLoginDto CreateUserLoginDto()
        {
            return new UserLoginDto
            {
                Email = "user@test.com",
                Password = "TestPassword@123"
            };
        }

        public static UserUpdateDto CreateUserUpdateDto(string? fullName = "Updated Name", string? email = "updated@test.com")
        {
            return new UserUpdateDto
            {
                FullName = fullName,
                Email = email,
                IsApproved = true
            };
        }

        // ===== PROPERTY FIXTURES =====
        public static Property CreateTestProperty(int id = 1, int ownerId = 1, int createdBy = 1, bool isApproved = false)
        {
            return new Property
            {
                PropertyId = id,
                PropertyNumber = $"PROP-{id:000}",
                Title = $"Test Property {id}",
                Description = "Test Description",
                Address = "123 Test Street",
                City = "Test City",
                Area = 1000,
                Price = 500000,
                OwnerId = ownerId,
                CreatedBy = createdBy,
                IsApproved = isApproved,
                IsAvailable = true,
                Status = isApproved ? "Approved" : "Pending",
                CreatedAt = DateTime.UtcNow
            };
        }

        public static PropertyCreateDto CreatePropertyCreateDto(int ownerId = 1, int createdBy = 1, IEnumerable<IFormFile>? documents = null)
        {
            return new PropertyCreateDto
            {
                Title = "New Property",
                Description = "New Test Description",
                Address = "456 New Street",
                City = "New City",
                Area = 1500,
                Price = 750000,
                OwnerId = ownerId,
                CreatedBy = createdBy,
                Documents = documents ?? new List<IFormFile> { CreateFormFile() }
            };
        }

        public static IFormFile CreateFormFile(string fileName = "test.pdf", string content = "Sample content")
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            stream.Position = 0;

            return new FormFile(stream, 0, bytes.Length, "documents", fileName)
            {
                Headers = new HeaderDictionary
                {
                    [HeaderNames.ContentType] = GetContentType(fileName)
                },
                ContentType = GetContentType(fileName)
            };
        }

        private static string GetContentType(string fileName)
        {
            return Path.GetExtension(fileName).ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }

        public static PropertyUpdateDto CreatePropertyUpdateDto(string? title = "Updated Property", decimal? price = 600000)
        {
            return new PropertyUpdateDto
            {
                Title = title,
                Description = "Updated Description",
                Address = "789 Updated Street",
                City = "Updated City",
                Area = 2000,
                Price = price,
                IsAvailable = true,
                Remarks = "Updated remarks"
            };
        }

        public static PropertyApprovalDto CreatePropertyApprovalDto(int propertyId = 1, bool approve = true, int adminId = 1)
        {
            return new PropertyApprovalDto
            {
                PropertyId = propertyId,
                Approve = approve,
                AdminId = adminId,
                Remarks = approve ? "Looks good" : "Needs more info"
            };
        }

        public static PropertyResponseDto CreatePropertyResponseDto(int id = 1, int ownerId = 1)
        {
            return new PropertyResponseDto
            {
                PropertyId = id,
                PropertyNumber = $"PROP-{id:000}",
                Title = $"Test Property {id}",
                Description = "Test Description",
                Address = "123 Test Street",
                City = "Test City",
                Area = 1000,
                Price = 500000,
                IsApproved = false,
                IsAvailable = true,
                Status = "Pending",
                Remarks = null,
                OwnerId = ownerId,
                CreatedBy = 1,
                CreatedAt = DateTime.UtcNow
            };
        }

        // ===== ADMIN DTO FIXTURES =====
        public static ApproveAgentDto CreateApproveAgentDto(bool approve = true)
        {
            return new ApproveAgentDto
            {
                Approve = approve,
                Remarks = approve ? "Agent approved" : "Agent rejected"
            };
        }

        public static ApprovePropertyDto CreateApprovePropertyDto(bool approve = true)
        {
            return new ApprovePropertyDto
            {
                Approve = approve,
                Remarks = approve ? "Property approved" : "Property rejected"
            };
        }

        public static AgentApprovalResponseDto CreateAgentApprovalResponseDto(int agentId = 1, bool isApproved = false)
        {
            return new AgentApprovalResponseDto
            {
                AgentId = agentId,
                FullName = $"Agent {agentId}",
                Email = $"agent{agentId}@test.com",
                IsApproved = isApproved,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static PropertyApprovalResponseDto CreatePropertyApprovalResponseDto(int propertyId = 1, bool isApproved = false)
        {
            return new PropertyApprovalResponseDto
            {
                PropertyId = propertyId,
                Title = $"Test Property {propertyId}",
                Address = "123 Test Street",
                City = "Test City",
                OwnerName = "Test Owner",
                IsApproved = isApproved,
                CreatedAt = DateTime.UtcNow
            };
        }

        // ===== LIST FIXTURES =====
        public static List<Property> CreateTestPropertyList(int count = 3)
        {
            var list = new List<Property>();
            for (int i = 1; i <= count; i++)
            {
                list.Add(CreateTestProperty(i, i, 1, i % 2 == 0));
            }
            return list;
        }

        public static List<PropertyResponseDto> CreatePropertyResponseDtoList(int count = 3)
        {
            var list = new List<PropertyResponseDto>();
            for (int i = 1; i <= count; i++)
            {
                list.Add(CreatePropertyResponseDto(i, i));
            }
            return list;
        }

        public static List<User> CreateTestUserList(int count = 3)
        {
            var list = new List<User>();
            for (int i = 1; i <= count; i++)
            {
                list.Add(CreateTestUser(i, $"user{i}@test.com", i % 2 == 0 ? "Agent" : "User"));
            }
            return list;
        }

        // ===== AGENT PROPERTY FIXTURES =====
        public static AgentProperty CreateTestAgentProperty(int id = 1, int agentId = 1, int propertyId = 1, bool isApproved = false)
        {
            return new AgentProperty
            {
                AgentPropertyId = id,
                AgentId = agentId,
                PropertyId = propertyId,
                AssignedDate = DateTime.UtcNow,
                Status = isApproved ? "Active" : "Pending",
                IsApproved = isApproved,
                Notes = "Test Assignment"
            };
        }

        public static AgentPropertyCreateDto CreateAgentPropertyCreateDto(int agentId = 1, int propertyId = 1, string? notes = "Test notes")
        {
            return new AgentPropertyCreateDto
            {
                AgentId = agentId,
                PropertyId = propertyId,
                Notes = notes
            };
        }

        public static AgentPropertyApproveDto CreateAgentPropertyApproveDto(int agentPropertyId = 1, bool approve = true)
        {
            return new AgentPropertyApproveDto
            {
                AgentPropertyId = agentPropertyId,
                Approve = approve
            };
        }

        public static AgentPropertyResponseDto CreateAgentPropertyResponseDto(int id = 1, int agentId = 1, int propertyId = 1)
        {
            return new AgentPropertyResponseDto
            {
                AgentPropertyId = id,
                AgentId = agentId,
                AgentName = $"Agent {agentId}",
                PropertyId = propertyId,
                PropertyTitle = $"Property {propertyId}",
                Status = "Pending",
                IsApproved = false,
                AssignedDate = DateTime.UtcNow,
                Notes = "Test Assignment"
            };
        }

        // ===== DOCUMENT FIXTURES =====
        public static Document CreateTestDocument(int id = 1, int propertyId = 1, int uploadedBy = 1, bool verified = false)
        {
            return new Document
            {
                DocumentId = id,
                PropertyId = propertyId,
                UploadedBy = uploadedBy,
                DocumentType = "PropertyDoc",
                FileName = $"Document_{id}.pdf",
                FilePath = $"/uploads/doc_{id}.pdf",
                UploadDate = DateTime.UtcNow,
                Verified = verified,
                VerifiedBy = verified ? 2 : null,
                VerifiedDate = verified ? DateTime.UtcNow : null
            };
        }

        public static DocumentCreateDto CreateDocumentCreateDto(int propertyId = 1, int uploadedBy = 1, string? docName = "TestDoc.pdf")
        {
            return new DocumentCreateDto
            {
                PropertyId = propertyId,
                UploadedBy = uploadedBy,
                DocumentName = docName ?? "TestDoc.pdf",
                DocumentType = "PropertyDoc"
            };
        }

        public static DocumentVerifyDto CreateDocumentVerifyDto(int documentId = 1, int verifierId = 2, bool verified = true)
        {
            return new DocumentVerifyDto
            {
                DocumentId = documentId,
                VerifierId = verifierId,
                Verified = verified
            };
        }

        public static DocumentResponseDto CreateDocumentResponseDto(int id = 1, int propertyId = 1, int uploadedBy = 1)
        {
            return new DocumentResponseDto
            {
                DocumentId = id,
                PropertyId = propertyId,
                PropertyTitle = $"Property {propertyId}",
                FileName = $"Document_{id}.pdf",
                DocumentType = "PropertyDoc",
                FilePath = $"/uploads/doc_{id}.pdf",
                UploadedBy = uploadedBy,
                UploaderName = $"User {uploadedBy}",
                Verified = false,
                UploadDate = DateTime.UtcNow
            };
        }

        // ===== PROPERTY OWNERSHIP FIXTURES =====
        public static PropertyOwnership CreateTestPropertyOwnership(int id = 1, int propertyId = 1, int userId = 1, bool verified = false)
        {
            return new PropertyOwnership
            {
                OwnershipId = id,
                PropertyId = propertyId,
                UserId = userId,
                OwnershipType = "Primary",
                StartDate = DateTime.UtcNow,
                Verified = verified,
                Status = verified ? "Approved" : "Pending",
                VerifiedBy = verified ? 2 : null,
                VerifiedDate = verified ? DateTime.UtcNow : null
            };
        }

        public static PropertyOwnershipCreateDto CreatePropertyOwnershipCreateDto(int propertyId = 1, int ownerId = 1)
        {
            return new PropertyOwnershipCreateDto
            {
                PropertyId = propertyId,
                OwnerId = ownerId,
                OwnershipStartDate = DateTime.UtcNow
            };
        }

        public static PropertyOwnershipVerifyDto CreatePropertyOwnershipVerifyDto(int ownershipId = 1, int verifierId = 2, bool verified = true)
        {
            return new PropertyOwnershipVerifyDto
            {
                OwnershipId = ownershipId,
                VerifierId = verifierId,
                Verified = verified
            };
        }

        public static PropertyOwnershipResponseDto CreatePropertyOwnershipResponseDto(int id = 1, int propertyId = 1, int ownerId = 1)
        {
            return new PropertyOwnershipResponseDto
            {
                OwnershipId = id,
                PropertyId = propertyId,
                PropertyTitle = $"Property {propertyId}",
                OwnerId = ownerId,
                OwnerName = $"User {ownerId}",
                OwnershipType = "Primary",
                Verified = false,
                Status = "Pending",
                OwnershipStartDate = DateTime.UtcNow
            };
        }

        // ===== PROPERTY TRANSACTION FIXTURES =====
        public static PropertyTransaction CreateTestPropertyTransaction(int id = 1, int propertyId = 1, int sellerId = 1, int buyerId = 2, int? agentId = null, string status = "Pending")
        {
            return new PropertyTransaction
            {
                TransactionId = id,
                PropertyId = propertyId,
                SellerId = sellerId,
                BuyerId = buyerId,
                AgentId = agentId,
                Amount = 500000,
                AgentCommission = agentId.HasValue ? 25000 : null,
                TransactionDate = DateTime.UtcNow,
                Status = status,
                VerifiedBy = status == "Verified" ? 1 : null,
                VerifiedDate = status == "Verified" ? DateTime.UtcNow : null
            };
        }

        public static PropertyTransactionCreateDto CreatePropertyTransactionCreateDto(int propertyId = 1, int sellerId = 1, int buyerId = 2, int? agentId = null)
        {
            return new PropertyTransactionCreateDto
            {
                PropertyId = propertyId,
                SellerId = sellerId,
                BuyerId = buyerId,
                AgentId = agentId,
                TransactionAmount = 500000,
                AgentCommission = agentId.HasValue ? 25000 : null,
                TransactionDate = DateTime.UtcNow
            };
        }

        public static PropertyTransactionVerifyDto CreatePropertyTransactionVerifyDto(int transactionId = 1, int verifierId = 1, bool approve = true)
        {
            return new PropertyTransactionVerifyDto
            {
                TransactionId = transactionId,
                VerifierId = verifierId,
                Approve = approve
            };
        }

        public static PropertyTransactionResponseDto CreatePropertyTransactionResponseDto(int id = 1, int propertyId = 1, int sellerId = 1, int buyerId = 2)
        {
            return new PropertyTransactionResponseDto
            {
                TransactionId = id,
                PropertyId = propertyId,
                PropertyTitle = $"Property {propertyId}",
                SellerId = sellerId,
                SellerName = $"User {sellerId}",
                BuyerId = buyerId,
                BuyerName = $"User {buyerId}",
                TransactionAmount = 500000,
                Status = "Pending",
                TransactionDate = DateTime.UtcNow
            };
        }
    }
}
