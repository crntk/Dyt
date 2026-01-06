using System;

namespace Dyt.Contracts.About.Responses
{
 /// <summary>
    /// Sertifika/Baþarý için Response DTO
    /// </summary>
    public class CertificateDto
    {
        public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Issuer { get; set; } = string.Empty;
        public DateTime? IssueDate { get; set; }
    public string? Description { get; set; }
  public string? ImageUrl { get; set; }
 public string? VerificationUrl { get; set; }
 public int DisplayOrder { get; set; }
        public bool IsPublished { get; set; }
 public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
}
}
