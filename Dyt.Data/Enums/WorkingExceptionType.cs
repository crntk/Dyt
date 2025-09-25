using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Data.Enums // Enum'ların ortak ad alanı
{
    /// <summary>
    /// Çalışma saatleri istisnasının tipini temsil eder.
    /// </summary>
    public enum WorkingExceptionType // İstisna türleri
    {
        Closed = 0, // O gün/saat aralığı kapalı
        ExtraOpen = 1 // Normalde kapalı iken ekstra açık
    }
}

