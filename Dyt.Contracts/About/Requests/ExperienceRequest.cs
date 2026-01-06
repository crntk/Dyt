using System;
using System.ComponentModel.DataAnnotations;

namespace Dyt.Contracts.About.Requests
{
    /// <summary>
    /// Deneyim/Özgeçmiþ için oluþtur/güncelle request
    /// </summary>
  public class ExperienceRequest
    {
 [Required(ErrorMessage = "Pozisyon/Ünvan zorunludur")]
  [StringLength(200, ErrorMessage = "Pozisyon en fazla 200 karakter olabilir")]
  public string Position { get; set; } = string.Empty;

   [Required(ErrorMessage = "Kurum/Okul adý zorunludur")]
 [StringLength(200, ErrorMessage = "Kurum adý en fazla 200 karakter olabilir")]
  public string Institution { get; set; } = string.Empty;

   [StringLength(1000, ErrorMessage = "Açýklama en fazla 1000 karakter olabilir")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Baþlangýç tarihi zorunludur")]
        public DateTime StartDate { get; set; }

  public DateTime? EndDate { get; set; }

public bool IsCurrent { get; set; }

  [Required]
     [RegularExpression("^(Work|Education|Internship)$", ErrorMessage = "Geçerli bir tip seçiniz")]
        public string Type { get; set; } = "Work";

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
