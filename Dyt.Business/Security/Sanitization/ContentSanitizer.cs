using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions; // Regex ile basit temizleme yapabilmek için ekliyorum

namespace Dyt.Business.Security.Sanitization // İçerik temizleme için ad alanını belirliyorum
{
    /// <summary>
    /// Kullanıcıdan gelen metinleri güvenli hale getirmek için basit bir temizleyici.
    /// Markdown destekleniyorsa HTML'e dönüştürmeden önce çalıştırılabilir.
    /// Bu basit uygulama script/style/iframe gibi tehlikeli tag'ları ve event handler attribute'larını temizler.
    /// </summary>
    public class ContentSanitizer : IContentSanitizer // Arayüzü uygulayan sınıfı oluşturuyorum
    {
        // Tehlikeli HTML tag'larını yakalayan regex'i tanımlıyorum
        private static readonly Regex DangerousTags = new Regex(@"</?(script|style|iframe|object|embed|link|meta)[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled); // Tehlikeli etiketleri tespit eden desen

        // onload, onclick gibi event handler attribute'larını yakalayan regex'i tanımlıyorum
        private static readonly Regex DangerousAttrs = new Regex(@"\son\w+\s*=\s*[""'][^""']*[""']", RegexOptions.IgnoreCase | RegexOptions.Compiled); // Olay niteliklerini tespit eden desen

        /// <summary>
        /// Girilen metni basit kurallarla temizler.
        /// </summary>
        public string Sanitize(string input) // Temizleme metodunu tanımlıyorum
        {
            if (string.IsNullOrEmpty(input)) // Boşsa
                return string.Empty; // Boş string döndürüyorum

            var s = DangerousTags.Replace(input, string.Empty); // Tehlikeli etiketleri tamamen kaldırıyorum
            s = DangerousAttrs.Replace(s, string.Empty); // Olay niteliklerini temizliyorum
            return s; // Son halini döndürüyorum
        }
    }
}

