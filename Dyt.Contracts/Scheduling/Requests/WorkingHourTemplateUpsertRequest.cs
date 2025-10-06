using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations; // Doğrulama öznitelikleri için ekliyorum

namespace Dyt.Contracts.Scheduling.Requests // Scheduling istek modelleri için ad alanı
{
    /// <summary>
    /// Çalışma saatleri şablonu satırı oluşturma/güncelleme isteği.
    /// </summary>
    public class WorkingHourTemplateUpsertRequest // Upsert isteğini tanımlıyorum
    {
        public int? Id { get; set; } // Güncellemede dolu, oluşturma için null bırakıyorum

        [Range(0, 6)] // Gün değeri 0-6 aralığında olmalı
        public int DayOfWeek { get; set; } // Haftanın gününü alıyorum

        [Required] // Zorunlu alan olduğunu belirtiyorum
        public TimeOnly StartTime { get; set; } // Başlangıç saatini alıyorum

        [Required] // Zorunlu alan olduğunu belirtiyorum
        public TimeOnly EndTime { get; set; } // Bitiş saatini alıyorum

        public bool IsActive { get; set; } = true; // Varsayılanı aktif yapıyorum
    }
}

