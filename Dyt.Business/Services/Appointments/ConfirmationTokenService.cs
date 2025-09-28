using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography; // HMACSHA256 için gerekli
using System.Text; // UTF8 kodlama için
using Dyt.Business.Interfaces.Appointments; // Arayüzü uygulamak için
using Dyt.Business.Options; // SecurityOptions'a erişmek için
using Microsoft.Extensions.Options; // IOptions pattern için

namespace Dyt.Business.Services.Appointments // Servis implementasyonlarının bulunduğu ad alanı
{
    /// <summary>
    /// Randevu onay/ret linkleri için tek kullanımlık, süreli ve imzalı token üretimi/çözümü.
    /// Token formatı: base64url(appointmentId|intent|expiresUnix|HMAC)
    /// </summary>
    public class ConfirmationTokenService : IConfirmationTokenService // Arayüz uygulaması
    {
        private readonly SecurityOptions _opt; // Güvenlik seçeneklerini tutuyorum

        /// <summary>
        /// Güvenlik ayarlarını alarak servisi başlatır.
        /// </summary>
        public ConfirmationTokenService(IOptions<SecurityOptions> opt) // IOptions ile ayarları alıyorum
        {
            _opt = opt.Value; // Değeri alan değişkenine atıyorum
        }

        /// <summary>
        /// "Gelecek" intentli token üretir.
        /// </summary>
        public string GenerateYesToken(int appointmentId, DateTimeOffset expiresAtUtc) // Evet token üretimi
            => GenerateToken(appointmentId, "YES", expiresAtUtc); // Ortak üretim metoduna delege ediyorum

        /// <summary>
        /// "Gelmeyecek" intentli token üretir.
        /// </summary>
        public string GenerateNoToken(int appointmentId, DateTimeOffset expiresAtUtc) // Hayır token üretimi
            => GenerateToken(appointmentId, "NO", expiresAtUtc); // Ortak üretim metoduna delege ediyorum

        /// <summary>
        /// Verilen token'ı doğrular ve randevu kimliğini çıkarır.
        /// </summary>
        public (bool IsValid, int AppointmentId) Validate(string token) // Doğrulama metodu
        {
            try // Hataları yakalamak için try bloğu açıyorum
            {
                var raw = Base64UrlDecode(token); // Token'ı base64url'den ham string'e çeviriyorum
                var parts = raw.Split('|'); // Ayırıcı karaktere göre parçalıyorum
                if (parts.Length != 4) return (false, 0); // Parça sayısı beklenen değilse geçersiz

                var id = int.Parse(parts[0]); // İlk parça randevu kimliği
                var intent = parts[1]; // İkinci parça intent (YES/NO)
                var expUnix = long.Parse(parts[2]); // Üçüncü parça son kullanma zamanı (unix saniye)
                var sig = parts[3]; // Dördüncü parça imza

                var exp = DateTimeOffset.FromUnixTimeSeconds(expUnix); // Unix saniyeyi tarih zamanına çeviriyorum
                if (DateTimeOffset.UtcNow > exp) return (false, 0); // Süresi dolmuşsa geçersiz

                var expectedSig = Sign($"{id}|{intent}|{expUnix}"); // Aynı veriyle beklenen imzayı üretiyorum
                if (!CryptographicOperations.FixedTimeEquals( // Zaman sabit karşılaştırma ile
                        Encoding.UTF8.GetBytes(sig), // Sağlanan imzayı
                        Encoding.UTF8.GetBytes(expectedSig))) // Beklenenle kıyaslıyorum
                    return (false, 0); // Eşleşmiyorsa geçersiz

                return (true, id); // Tüm kontroller geçerse geçerli ve kimlik değerini dönerim
            }
            catch // Pars etme ve benzeri hatalarda
            {
                return (false, 0); // Geçersiz token olarak sonuç dönerim
            }
        }

        /// <summary>
        /// Ortak token üretim yordamı.
        /// </summary>
        private string GenerateToken(int appointmentId, string intent, DateTimeOffset expiresAtUtc) // İç yardımcı metot
        {
            var payload = $"{appointmentId}|{intent}|{expiresAtUtc.ToUnixTimeSeconds()}"; // İmza öncesi ham veriyi oluşturuyorum
            var sig = Sign(payload); // İmzayı üretiyorum
            var packed = $"{payload}|{sig}"; // İmzayı payload'la birleştiriyorum
            return Base64UrlEncode(packed); // Base64url ile güvenli temsil oluşturup döndürüyorum
        }

        /// <summary>
        /// HMAC-SHA256 ile imza üretir.
        /// </summary>
        private string Sign(string data) // Veri imzalama yardımcı metodu
        {
            var key = Encoding.UTF8.GetBytes(_opt.SigningKey); // Gizli anahtarı byte dizisine çeviriyorum
            using var hmac = new HMACSHA256(key); // HMACSHA256 örneği oluşturuyorum
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data)); // Veriye imza atıyorum
            return Convert.ToHexString(hash); // Hex string olarak döndürüyorum
        }

        /// <summary>
        /// Base64url encode işlemi (RFC 4648).
        /// </summary>
        private static string Base64UrlEncode(string input) // Base64url kodlama
        {
            var bytes = Encoding.UTF8.GetBytes(input); // Girdiyi byte dizisine çeviriyorum
            return Convert.ToBase64String(bytes) // Standart base64 oluşturuyorum
                .TrimEnd('=') // Eşittir dolgularını kesiyorum
                .Replace('+', '-') // Artı işaretlerini tire ile değiştiriyorum
                .Replace('/', '_'); // Slashları alt çizgi ile değiştiriyorum
        }

        /// <summary>
        /// Base64url decode işlemi (RFC 4648).
        /// </summary>
        private static string Base64UrlDecode(string input) // Base64url çözme
        {
            var padded = input.Replace('-', '+').Replace('_', '/'); // URL güvenli karakterleri geri çeviriyorum
            switch (padded.Length % 4) // Padding durumunu kontrol ediyorum
            {
                case 2: padded += "=="; break; // 2 eksik padding varsa iki eşittir ekliyorum
                case 3: padded += "="; break; // 1 eksik padding varsa bir eşittir ekliyorum
            }
            var bytes = Convert.FromBase64String(padded); // Base64'ü byte dizisine çeviriyorum
            return Encoding.UTF8.GetString(bytes); // Byte dizisini string'e çevirip döndürüyorum
        }
    }
}

