using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Contracts.Scheduling.Responses // Scheduling yanıt modelleri için ad alanı
{
    /// <summary>
    /// Tarih bazlı özel durum (kapalı gün veya ekstra açık saat) kaydı DTO’su.
    /// </summary>
    public class WorkingHourExceptionDto // DTO sınıfını tanımlıyorum
    {
        public int Id { get; set; } // Kayıt kimliğini tutuyorum
        public DateOnly Date { get; set; } // Gün bilgisini tutuyorum
        public TimeOnly? StartTime { get; set; } // Başlangıç saatini (boş ise tüm gün) tutuyorum
        public TimeOnly? EndTime { get; set; } // Bitiş saatini (boş ise tüm gün) tutuyorum
        public bool IsClosed { get; set; } // Bu kaydın kapalı günü mü ifade ettiğini tutuyorum
        public string? Note { get; set; } // Açıklama notunu tutuyorum
    }
}

