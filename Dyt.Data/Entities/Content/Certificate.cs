using System;
using Dyt.Data.Entities.Common;

namespace Dyt.Data.Entities.Content
{
    /// <summary>
    /// Baþarýlar & Sertifikalar bölümü için entity
    /// </summary>
  public class Certificate : AuditableEntity
    {
        /// <summary>
        /// Sertifika/Baþarý baþlýðý
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Veren kuruluþ
     /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
    /// Alýndýðý tarih
     /// </summary>
 public DateTime? IssueDate { get; set; }

        /// <summary>
   /// Açýklama (opsiyonel)
        /// </summary>
        public string? Description { get; set; }

/// <summary>
        /// Sertifika görseli URL'si (opsiyonel)
        /// </summary>
   public string? ImageUrl { get; set; }

    /// <summary>
    /// Doðrulama linki (opsiyonel)
   /// </summary>
        public string? VerificationUrl { get; set; }

        /// <summary>
        /// Gösterim sýrasý (küçükten büyüðe sýralanýr)
        /// </summary>
 public int DisplayOrder { get; set; }

        /// <summary>
        /// Aktif/Pasif durumu
 /// </summary>
        public bool IsPublished { get; set; } = true;
    }
}
