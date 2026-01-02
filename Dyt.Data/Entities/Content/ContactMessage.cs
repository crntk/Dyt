using System;
using Dyt.Data.Entities.Common;

namespace Dyt.Data.Entities.Content
{
 /// <summary>
    /// Ýletiþim formundan gelen mesajlar
    /// </summary>
    public class ContactMessage : AuditableEntity
    {
      /// <summary>
   /// Gönderen adý soyadý
        /// </summary>
      public string Name { get; set; } = string.Empty;

        /// <summary>
   /// Gönderen e-posta adresi
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
      /// Gönderen telefon numarasý
        /// </summary>
      public string Phone { get; set; } = string.Empty;

      /// <summary>
 /// Mesaj konusu
        /// </summary>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Mesaj içeriði
        /// </summary>
  public string Message { get; set; } = string.Empty;

        /// <summary>
        /// KVKK onayý verildi mi
        /// </summary>
        public bool KvkkConsent { get; set; }

        /// <summary>
        /// Mesaj okundu mu
        /// </summary>
        public bool IsRead { get; set; } = false;

      /// <summary>
      /// Mesaja yanýt verildi mi
        /// </summary>
        public bool IsReplied { get; set; } = false;

        /// <summary>
  /// Admin notu (iç kullaným için)
        /// </summary>
        public string? AdminNote { get; set; }

      /// <summary>
        /// Mesajýn okunma tarihi
        /// </summary>
        public DateTime? ReadAt { get; set; }

        /// <summary>
    /// Yanýt verilme tarihi
        /// </summary>
        public DateTime? RepliedAt { get; set; }
    }
}
