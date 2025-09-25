using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Common; // AuditableEntity için

namespace Dyt.Data.Entities.Settings // Ayar varlıklarının ad alanı
{
    /// <summary>
    /// SMS içerik şablonlarını saklar. Metin içinde yer tutucular kullanılabilir.
    /// </summary>
    public class SmsTemplate : AuditableEntity // Audit ve soft-delete alanları için kalıtım
    {
        public string TemplateKey { get; set; } = string.Empty; // Şablon anahtarı (ör. Reminder)

        public string Content { get; set; } = string.Empty; // Şablon metni
    }
}

