using System;

namespace APIPropertyRegistry.DTOs
{
    public class DocumentCreateDto
    {
        public int PropertyId { get; set; }
        public int UploadedBy { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string? DocumentName { get; set; }
    }

    public class DocumentVerifyDto
    {
        public int DocumentId { get; set; }
        public int VerifierId { get; set; }
        public bool Verified { get; set; }
    }

    public class DocumentResponseDto
    {
        public int DocumentId { get; set; }
        public int PropertyId { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int UploadedBy { get; set; }
        public string UploaderName { get; set; } = string.Empty;
        public bool Verified { get; set; }
        public int? VerifiedBy { get; set; }
        public string? VerifierName { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime? VerifiedDate { get; set; }
    }
}
