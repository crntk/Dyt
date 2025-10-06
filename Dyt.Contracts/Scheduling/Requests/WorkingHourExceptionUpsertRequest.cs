using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations; // Doğrulama için ekliyorum

namespace Dyt.Contracts.Scheduling.Requests // Scheduling istek modelleri için ad alanı
{
    /// <summary>
    /// Tarih bazlı istisna (kapalı/ekstra açık) oluşturma-güncelleme isteği.
    /// </summary>
    public class WorkingHourExceptionUpsertRequest // Upsert isteğini tanımlıyorum
    {
        public int? Id { get; set; } // Güncellemede dolu olabilir

        [Required] // Zorunlu alan
        public DateOnly Date { get; set; } // Gün bilgisini alıyorum

        public TimeOnly? StartTime { get; set; } // Saat aralığı başlangıcı opsiyonel
        public TimeOnly? EndTime { get; set; } // Saat aralığı bitişi opsiyonel

        public bool IsClosed { get; set; } // Kapalı gün mü işareti

        [StringLength(200)] // Not uzunluğunu sınırlıyorum
        public string? Note { get; set; } // Açıklama
    }
}

