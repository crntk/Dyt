using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Common; // AuditableEntity için

namespace Dyt.Data.Entities.Settings // Ayar varlıklarının ad alanı
{
    /// <summary>
    /// Basit anahtar-değer sistemi ile genel uygulama ayarları.
    /// </summary>
    public class SystemSetting : AuditableEntity // Audit ve soft-delete alanları için kalıtım
    {
        public string Key { get; set; } = string.Empty; // Ayar anahtarı (benzersiz olacak)

        public string Value { get; set; } = string.Empty; // Ayar değeri (string saklanır)
    }
}

