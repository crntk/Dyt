using System;
using System.ComponentModel.DataAnnotations;

namespace Dyt.Contracts.About.Requests
{
    /// <summary>
    /// Sertifika/Baþarý için oluþtur/güncelle request
    /// </summary>
    public class CertificateRequest
    {
  [Required(ErrorMessage = "Sertifika/Baþarý baþlýðý zorunludur")]
        [StringLength(300, ErrorMessage = "Baþlýk en fazla 300 karakter olabilir")]
public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Veren kuruluþ zorunludur")]
   [StringLength(200, ErrorMessage = "Kuruluþ adý en fazla 200 karakter olabilir")]
        public string Issuer { get; set; } = string.Empty;

 public DateTime? IssueDate { get; set; }

        [StringLength(1000, ErrorMessage = "Açýklama en fazla 1000 karakter olabilir")]
        public string? Description { get; set; }

[StringLength(500)]
      public string? ImageUrl { get; set; }

     [StringLength(500)]
        [Url(ErrorMessage = "Geçerli bir URL girin")]
 public string? VerificationUrl { get; set; }

 public int DisplayOrder { get; set; }

        public bool IsPublished { get; set; } = true;
    }
}
