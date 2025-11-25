using APIPropertyRegistry.DTOs;
using APIPropertyRegistry.Models;
using APIPropertyRegistry.Repositories.Interfaces;
using APIPropertyRegistry.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using APIPropertyRegistry.Data;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Services.Implementations
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _repository;
        private readonly ApplicationDbContext _context;
        private readonly Cloudinary _cloudinary;
        private readonly string _cloudinaryFolder = "property_documents";

        public DocumentService(IDocumentRepository repository, ApplicationDbContext context, IConfiguration configuration)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _context = context ?? throw new ArgumentNullException(nameof(context));

            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];
            if (string.IsNullOrWhiteSpace(cloudName) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
                throw new InvalidOperationException("Cloudinary configuration missing (CloudName/ApiKey/ApiSecret).");

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account)
            {
                Api = { Secure = true }
            };
        }

        public async Task<IEnumerable<DocumentResponseDto>> GetAllAsync()
        {
            var docs = await _repository.GetAllAsync();
            return docs.Select(MapToResponse);
        }

        public async Task<IEnumerable<DocumentResponseDto>> GetPendingAsync()
        {
            var docs = await _repository.GetPendingAsync();
            return docs.Select(MapToResponse);
        }

        public async Task<IEnumerable<DocumentResponseDto>> GetByPropertyAsync(int propertyId)
        {
            var docs = await _repository.GetByPropertyAsync(propertyId);
            return docs.Select(MapToResponse);
        }

        public async Task<DocumentResponseDto?> GetByIdAsync(int id)
        {
            var doc = await _repository.GetByIdAsync(id);
            return doc == null ? null : MapToResponse(doc);
        }

        public async Task<DocumentResponseDto> CreateAsync(DocumentCreateDto dto, IFormFile file)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (file == null || file.Length == 0) throw new ArgumentException("A valid file must be provided.", nameof(file));

            var uploader = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == dto.UploadedBy);
            if (uploader == null) throw new InvalidOperationException("Uploader (user) not found.");

            var property = await _context.Properties.AsNoTracking().FirstOrDefaultAsync(p => p.PropertyId == dto.PropertyId);
            if (property == null) throw new InvalidOperationException("Property not found.");

            var uniqueId = Guid.NewGuid().ToString();
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                PublicId = uniqueId,
                Folder = _cloudinaryFolder,
                Overwrite = false,
                UseFilename = false,
                UniqueFilename = true
            };

            RawUploadResult uploadResult;
            try
            {
                uploadResult = await _cloudinary.UploadAsync(uploadParams) as RawUploadResult;
            }
            catch (Exception ex)
            {
                throw new Exception("Cloudinary upload failed.", ex);
            }

            if (uploadResult == null)
                throw new Exception("Cloudinary did not return an upload result.");

            if (uploadResult.Error != null)
                throw new Exception($"Cloudinary error: {uploadResult.Error.Message}");

            var document = new Document
            {
                PropertyId = dto.PropertyId,
                UploadedBy = dto.UploadedBy,
                DocumentType = string.IsNullOrWhiteSpace(dto.DocumentType) ? "General" : dto.DocumentType.Trim(),
                FileName = string.IsNullOrWhiteSpace(dto.DocumentName) ? file.FileName : dto.DocumentName!,
                FilePath = uploadResult.SecureUrl?.ToString() ?? uploadResult.Url?.ToString() ?? string.Empty,
                UploadDate = DateTime.UtcNow,
                Verified = false
            };

            var saved = await _repository.AddAsync(document);

            return MapToResponse(saved);
        }

        public async Task<bool> VerifyAsync(DocumentVerifyDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            var doc = await _repository.GetByIdAsync(dto.DocumentId);
            if (doc == null) return false;

            doc.Verified = dto.Verified;
            doc.VerifiedBy = dto.VerifierId;
            doc.VerifiedDate = DateTime.UtcNow;

            return await _repository.UpdateAsync(doc);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var doc = await _repository.GetByIdAsync(id);
            if (doc == null) return false;

            try
            {
                var publicId = ExtractPublicIdFromUrl(doc.FilePath);
                if (!string.IsNullOrEmpty(publicId))
                {
                    var deletionParams = new DeletionParams(publicId)
                    {
                        ResourceType = ResourceType.Raw // documents uploaded as raw
                    };
                    var result = await _cloudinary.DestroyAsync(deletionParams);
                }
            }
            catch
            {
               
            }

            return await _repository.DeleteAsync(id);
        }


        private static string ExtractPublicIdFromUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return string.Empty;

            const string folderSegment = "property_documents";

            try
            {
                var uri = new Uri(url);
                var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length == 0) return string.Empty;

                var index = Array.FindIndex(segments, s => string.Equals(s, folderSegment, StringComparison.OrdinalIgnoreCase));
                if (index < 0) return string.Empty;

                var relevant = segments.Skip(index).ToArray();
                if (relevant.Length == 0) return string.Empty;

                var last = relevant[^1];
                var withoutExt = last.Contains('.') ? last.Substring(0, last.LastIndexOf('.')) : last;
                relevant[^1] = withoutExt;

                return string.Join('/', relevant);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static DocumentResponseDto MapToResponse(Document d)
        {
            return new DocumentResponseDto
            {
                DocumentId = d.DocumentId,
                PropertyId = d.PropertyId,
                PropertyTitle = d.Property?.Title ?? "Unknown",
                FileName = d.FileName,
                DocumentType = d.DocumentType,
                FilePath = d.FilePath ?? string.Empty,
                UploadedBy = d.UploadedBy,
                UploaderName = d.Uploader?.FullName ?? "Unknown",
                Verified = d.Verified,
                VerifiedBy = d.VerifiedBy,
                VerifierName = d.Verifier?.FullName,
                UploadDate = d.UploadDate,
                VerifiedDate = d.VerifiedDate
            };
        }
    }
}
