using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Data.Enums // Enum'ların ortak ad alanı
{
    /// <summary>
    /// SMS linki üzerinden danışanın verdiği yanıt durumunu ifade eder.
    /// </summary>
    public enum ConfirmationState // Onay/red (veya yanıtlanmadı) bilgisi
    {
        Yanıtlanmadı = 0, // Linke hiç yanıt vermedi
        Gelecek = 1, // Geleceğini bildirdi
        Gelmeyecek = 2  // Gelmeyeceğini bildirdi
    }
}

