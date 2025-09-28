using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Business.Options; // SecurityOptions ayarlarını okumak için ekliyorum
using Microsoft.Extensions.Options; // IOptions desenini kullanmak için ekliyorum

namespace Dyt.Business.Security.Url          // Arayüzle aynı namespace’i kullanıyorum
{
    /// <summary>
    /// BaseUrl + göreli path + query birleştiren yardımcı servis.
    /// Token imzalama ConfirmationTokenService'te; burada sadece URL’i kuruyorum.
    /// </summary>
    public class SignedUrlService : ISignedUrlService // Arayüzü uyguluyorum
    {
        private readonly SecurityOptions _opt; // Ayarları saklıyorum

        /// <summary>
        /// Güvenlik ayarlarını alarak servisi başlatır.
        /// </summary>
        public SignedUrlService(IOptions<SecurityOptions> opt) // IOptions deseni
        {
            _opt = opt.Value; // Değeri alan değişkenine atıyorum
        }

        /// <summary>
        /// BaseUrl + relative path + URL-encode edilmiş query string ile tam URL üretir.
        /// </summary>
        public string Build(string relativePath, IDictionary<string, string?> query) // URL üretim metodu
        {
            var baseUri = new Uri(_opt.BaseUrl.TrimEnd('/')); // BaseUrl'i Uri'ye çeviriyorum
            var path = relativePath.StartsWith('/') ? relativePath : "/" + relativePath; // Başında / yoksa ekliyorum

            var builder = new UriBuilder(new Uri(baseUri, path)); // Base + path'i birleştiriyorum

            // Query stringi el ile, güvenli şekilde üretiyorum
            var qs = new StringBuilder(); // Birleştirme için StringBuilder oluşturuyorum
            foreach (var kv in query) // Tüm parametreleri geziyorum
            {
                if (string.IsNullOrWhiteSpace(kv.Value)) // Değeri boş olanları atlıyorum
                    continue; // Sonraki elemana geçiyorum

                if (qs.Length > 0) qs.Append('&'); // İlkten sonraki parametrelerin önüne & koyuyorum
                qs.Append(Uri.EscapeDataString(kv.Key)); // Anahtarı URL güvenli hale getiriyorum
                qs.Append('='); // Eşittir koyuyorum
                qs.Append(Uri.EscapeDataString(kv.Value!)); // Değeri URL güvenli hale getiriyorum
            }

            builder.Query = qs.ToString(); // Ürettiğim query'yi builder'a veriyorum
            return builder.Uri.ToString(); // Tam URL'yi string olarak döndürüyorum
        }
    }
}


