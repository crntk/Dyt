using System.ComponentModel.DataAnnotations;

namespace Dyt.Contracts.About.Requests
{
    /// <summary>
    /// "Ben Kimim" bölümü için oluþtur/güncelle request
    /// </summary>
    public class AboutSectionRequest
    {
        [Required(ErrorMessage = "Baþlýk zorunludur")]
   [StringLength(200, ErrorMessage = "Baþlýk en fazla 200 karakter olabilir")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ýçerik zorunludur")]
 [StringLength(5000, ErrorMessage = "Ýçerik en fazla 5000 karakter olabilir")]
    public string ContentMarkdown { get; set; } = string.Empty;

        public bool IsPublished { get; set; } = true;
    }
}
