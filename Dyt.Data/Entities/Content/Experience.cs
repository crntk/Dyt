using System;
using System.ComponentModel.DataAnnotations;

namespace Dyt.Data.Entities.Content
{
    /// <summary>
    /// Diyetisyenin iþ deneyimleri ve eðitim geçmiþi
    /// </summary>
    public class Experience
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Pozisyon/Ünvan zorunludur")]
     [StringLength(200, ErrorMessage = "Pozisyon en fazla 200 karakter olabilir")]
      public string Position { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kurum/Okul adý zorunludur")]
     [StringLength(200, ErrorMessage = "Kurum adý en fazla 200 karakter olabilir")]
        public string Institution { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Açýklama en fazla 1000 karakter olabilir")]
        public string? Description { get; set; }

        /// <summary>
        /// Baþlangýç tarihi
        /// </summary>
        [Required(ErrorMessage = "Baþlangýç tarihi zorunludur")]
    public DateTime StartDate { get; set; }

        /// <summary>
        /// Bitiþ tarihi (null ise halen devam ediyor)
    /// </summary>
      public DateTime? EndDate { get; set; }

     /// <summary>
    /// Halen devam ediyor mu?
      /// </summary>
      public bool IsCurrent { get; set; }

        /// <summary>
  /// Deneyim tipi: Work (Ýþ), Education (Eðitim), Internship (Staj)
        /// </summary>
        [Required]
        [StringLength(50)]
   public string Type { get; set; } = "Work"; // Work, Education, Internship

        /// <summary>
        /// Gösterim sýrasý (küçükten büyüðe sýralanýr)
   /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
 /// Aktif/Pasif durumu
  /// </summary>
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
