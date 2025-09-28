using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Business.Interfaces.Appointments // Randevu onay token işlemleri için ad alanını tanımlıyorum
{
    /// <summary>
    /// Randevu onay/ret linkleri için güvenli, tek kullanımlık ve süreli token üretimi/çözümü.
    /// </summary>
    public interface IConfirmationTokenService // Onay token servisi arayüzü
    {
        string GenerateYesToken(int appointmentId, DateTimeOffset expiresAtUtc); // Gelecek (Evet) yanıtı için token üretimini bildiriyorum
        string GenerateNoToken(int appointmentId, DateTimeOffset expiresAtUtc); // Gelmeyecek (Hayır) yanıtı için token üretimini bildiriyorum
        (bool IsValid, int AppointmentId) Validate(string token); // Gelen token'ı doğrulayıp randevu kimliğini çıkartan metodu bildiriyorum
    }
}

