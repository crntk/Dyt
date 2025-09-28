using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Business.Options // Seçenek sınıfları için ad alanını tanımlıyorum
{
    /// <summary>
    /// SMS sağlayıcısına ait ayarlar. Canlıda gizli anahtarlar güvenli depodan okunmalıdır.
    /// </summary>
    public class SmsOptions // SMS yapılandırma seçeneklerini temsil eden sınıf
    {
        public const string SectionName = "Sms"; // appsettings içindeki bölüm adını sabitliyorum

        public string SenderName { get; set; } = "DIET"; // Mesaj başlığını/sender adını tutuyorum
        public string ProviderBaseUrl { get; set; } = string.Empty; // Sağlayıcı taban adresini tutuyorum
        public string ApiKey { get; set; } = string.Empty; // API anahtarını tutuyorum (gizli tutulmalı)
        public bool EnableRealSend { get; set; } = false; // Geliştirme ortamında gerçek gönderimi kapatmak için bayrak ekliyorum
    }
}

