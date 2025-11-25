using APIPropertyRegistry.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace APIPropertyRegistry.DTOs.PropertyDtos
{
    public class PropertyCreateDto
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public decimal Area { get; set; }
        public decimal Price { get; set; }
        public int OwnerId { get; set; }
        public int CreatedBy { get; set; }
        public int? AgentId { get; set; }
        public IEnumerable<IFormFile>? Documents { get; set; }
    }

    public class PropertyUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public decimal? Area { get; set; }
        public decimal? Price { get; set; }
        public bool? IsAvailable { get; set; }
        public string? Remarks { get; set; }
    }

    public class PropertyResponseDto
    {
        public int PropertyId { get; set; }
        public string PropertyNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public decimal Area { get; set; }
        public decimal Price { get; set; }
        public bool IsApproved { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsForSale { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public int OwnerId { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerEmail { get; set; }
        public int CreatedBy { get; set; }
        public int? AgentId { get; set; }
        public string? AgentName { get; set; }
        public string? AgentEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SaleListedDate { get; set; }
        public List<DocumentResponseDto> Documents { get; set; } = new();
    }

    public class PropertyApprovalDto
    {
        public int PropertyId { get; set; }
        public bool Approve { get; set; }
        public int AdminId { get; set; }
        public string? Remarks { get; set; }
    }

    public class PropertySellDto
    {
        public int PropertyId { get; set; }
        public int OwnerId { get; set; }
        public int AgentId { get; set; }
        public string? Remarks { get; set; }
    }
}
