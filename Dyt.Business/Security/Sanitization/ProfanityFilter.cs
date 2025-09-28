using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Business.Security.Sanitization // Küfür/hakaret filtresi için ad alanını belirliyorum
{
    /// <summary>
    /// Basit bir küfür/hakaret filtresi. Geliştirme sırasında küçük bir liste ile çalışır;
    /// canlıda listeyi veritabanı veya konfigürasyondan beslemek mümkündür.
    /// </summary>
    public class ProfanityFilter // Küfür filtresi sınıfını tanımlıyorum
    {
        private readonly HashSet<string> _blocklist; // Engellenecek kelimeleri tutan küme tanımlıyorum

        /// <summary>
        /// Varsayılan küçük bir kelime listesiyle başlatır; istenirse dışarıdan liste verilebilir.
        /// </summary>
        public ProfanityFilter(IEnumerable<string>? words = null) // Kurucu metot
        {
            _blocklist = new HashSet<string>( // HashSet ile hızlı arama sağlayan bir küme oluşturuyorum
                words ?? new[] { "küfür1", "küfür2", "hakaret1" }, // Varsayılan örnek kelimeleri veriyorum (canlıda gerçek liste kullanılacak)
                StringComparer.OrdinalIgnoreCase); // Büyük/küçük harf farkını önemsememek için karşılaştırıcı veriyorum
        }

        /// <summary>
        /// Girilen metinde engelli kelime geçip geçmediğini bildirir.
        /// </summary>
        public bool ContainsBannedWord(string input) // Kontrol metodunu tanımlıyorum
        {
            if (string.IsNullOrWhiteSpace(input)) // Boşsa
                return false; // Yasaklı kelime yok kabul ediyorum

            foreach (var w in _blocklist) // Tüm engelli kelimeleri geziyorum
            {
                if (input.Contains(w, StringComparison.OrdinalIgnoreCase)) // Metin içinde geçiyorsa
                    return true; // En az bir eşleşme olduğundan true döndürüyorum
            }
            return false; // Hiçbiri geçmediyse false döndürüyorum
        }
    }
}

