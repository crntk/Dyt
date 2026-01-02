using System;
using Dyt.Data.Entities.Common;

namespace Dyt.Data.Entities.Content
{
    /// <summary>
    /// Blog bülteni abonelik kaydý
    /// </summary>
    public class NewsletterSubscriber : AuditableEntity
    {
     /// <summary>
        /// Abone e-posta adresi
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
     /// Abonelik aktif mi
/// </summary>
     public bool IsActive { get; set; } = true;

        /// <summary>
        /// Abonelikten çýkmak için kullanýlan benzersiz token
        /// </summary>
        public string UnsubscribeToken { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
     /// E-posta onaylandý mý
        /// </summary>
        public bool IsVerified { get; set; } = true; // Basit versiyon için direkt onaylý

  /// <summary>
        /// Son gönderilen blog tarihi
        /// </summary>
   public DateTime? LastNotificationSentAt { get; set; }
    }
}
