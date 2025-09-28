using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Business.Options // Seçenek sınıfları için ad alanını tanımlıyorum
{
    /// <summary>
    /// Onay linkleri ve token güvenliği ile ilgili seçenekler.
    /// </summary>
    public class SecurityOptions // Güvenlik konfigürasyonlarını temsil eden sınıf
    {
        public const string SectionName = "Security"; // appsettings içindeki bölüm adını sabitliyorum

        public string BaseUrl { get; set; } = "https://example.com"; // Linklerin üretileceği temel adresi tutuyorum
        public int ConfirmationTokenTtlMinutes { get; set; } = 60 * 24; // Onay token'ının geçerlilik süresini dakika cinsinden tutuyorum
        public string SigningKey { get; set; } = string.Empty; // Token imzalamak için gizli anahtarı tutuyorum (gizli tutulmalı)
    }
}
