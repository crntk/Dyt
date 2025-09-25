using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Data.Enums // Enum'ların ortak ad alanı
{
    /// <summary>
    /// MediaFile kaydının hangi varlıkla ilişkili olduğunu anlatan tür.
    /// </summary>
    public enum MediaOwnerType // Medya sahiplik türleri
    {
        Profile = 0, // Diyetisyen profili
        BlogPost = 1, // Blog gönderisi
        Gallery = 2 // Opsiyonel başka galeriler
    }
}

