using System;

namespace APIPropertyRegistry.Models
{
public class Document
{
public int DocumentId { get; set; }
public int PropertyId { get; set; }
public int UploadedBy { get; set; }
public string DocumentType { get; set; } = string.Empty;
public string FileName { get; set; } = string.Empty;
public string FilePath { get; set; } = string.Empty;
public bool Verified { get; set; } = false;
public int? VerifiedBy { get; set; }
public DateTime? VerifiedDate { get; set; }
public DateTime UploadDate { get; set; } = DateTime.UtcNow;

public Property? Property { get; set; }
public User? Uploader { get; set; }
public User? Verifier { get; set; }
}
}