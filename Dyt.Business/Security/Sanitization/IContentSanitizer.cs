using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Business.Security.Sanitization
{
    /// <summary>
    /// HTML/Markdown çıktılarında temel XSS risklerini azaltmak için içerik temizleme sözleşmesi.
    /// </summary>
    public interface IContentSanitizer
    {
        /// <summary>
        /// Verilen HTML/markdown çıktısını güvenli hale getirir.
        /// </summary>
        string Sanitize(string input);
    }
}
