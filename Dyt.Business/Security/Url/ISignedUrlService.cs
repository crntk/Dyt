using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Dyt.Business/Security/Url/ISignedUrlService.cs

namespace Dyt.Business.Security.Url // Url servisleri için ad alanını sabit tutuyorum
{
    /// <summary>
    /// Dışa verilecek onay/ret linkleri gibi URL'leri üretmek için sözleşme.
    /// </summary>
    public interface ISignedUrlService // Arayüz tanımı
    {
        string Build(string relativePath, IDictionary<string, string?> query); // Path + query ile tam URL üretimini bildiriyorum
    }
}

