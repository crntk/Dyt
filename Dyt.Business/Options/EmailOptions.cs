namespace Dyt.Business.Options
{
    /// <summary>
    /// SMTP email gönderimi için yapýlandýrma ayarlarý
    /// </summary>
    public class EmailOptions
{
        public const string SectionName = "Email";

      /// <summary>
        /// SMTP sunucu adresi (örn: smtp.gmail.com)
     /// </summary>
    public string SmtpHost { get; set; } = string.Empty;

        /// <summary>
        /// SMTP sunucu portu (genellikle 587 veya 465)
        /// </summary>
        public int SmtpPort { get; set; } = 587;

        /// <summary>
        /// SMTP kullanýcý adý (email adresi)
        /// </summary>
        public string SmtpUsername { get; set; } = string.Empty;

        /// <summary>
     /// SMTP þifre veya uygulama þifresi
      /// </summary>
        public string SmtpPassword { get; set; } = string.Empty;

 /// <summary>
        /// Gönderen email adresi
   /// </summary>
     public string FromEmail { get; set; } = string.Empty;

        /// <summary>
        /// Gönderen adý
        /// </summary>
        public string FromName { get; set; } = "Diyetisyen Riyaza Tair";

        /// <summary>
    /// SSL/TLS kullan
      /// </summary>
  public bool EnableSsl { get; set; } = true;

        /// <summary>
        /// Gerçek email gönderimi aktif mi? (geliþtirme ortamýnda kapatýlabilir)
        /// </summary>
     public bool EnableRealSend { get; set; } = false;
    }
}
