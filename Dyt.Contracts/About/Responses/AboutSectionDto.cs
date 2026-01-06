using System;

namespace Dyt.Contracts.About.Responses
{
    /// <summary>
    /// "Ben Kimim" bölümü için Response DTO
  /// </summary>
    public class AboutSectionDto
    {
    public int Id { get; set; }
     public string Title { get; set; } = string.Empty;
    public string ContentMarkdown { get; set; } = string.Empty;
        /// <summary>
 /// Markdown içeriðinin HTML'e çevrilmiþ hali
        /// </summary>
        public string ContentHtml { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
    public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
