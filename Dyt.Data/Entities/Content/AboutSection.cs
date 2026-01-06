using System;
using Dyt.Data.Entities.Common;

namespace Dyt.Data.Entities.Content
{
    /// <summary>
    /// "Ben Kimim" bölümü için tek metin bloðu
    /// Sadece 1 kayýt olacak þekilde kullanýlýr
    /// </summary>
    public class AboutSection : AuditableEntity
    {
        /// <summary>
     /// Ben Kimim baþlýðý
        /// </summary>
      public string Title { get; set; } = "Ben Kimim?";

        /// <summary>
        /// Hakkýmda içerik metni (Markdown destekli)
        /// </summary>
        public string ContentMarkdown { get; set; } = string.Empty;

        /// <summary>
        /// Aktif/Pasif durumu
        /// </summary>
   public bool IsPublished { get; set; } = true;
    }
}
