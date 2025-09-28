using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Contracts.Settings;
using Dyt.Contracts.Settings.Responses; // Ayar DTO'larını kullanmak için ekliyorum

namespace Dyt.Business.Interfaces.Settings // Ayarlar servisleri için ad alanını tanımlıyorum
{
    /// <summary>
    /// Uygulama genel ayarlarının okunması ve güncellenmesi için servis sözleşmesi.
    /// </summary>
    public interface ISettingsService // Ayar servisi arayüzü
    {
        Task<SystemSettingDto?> GetAsync(string key, CancellationToken ct = default); // Anahtara göre ayar değeri getiren metodu bildiriyorum
        Task<bool> SetAsync(string key, string value, CancellationToken ct = default); // Anahtara göre ayar değerini güncelleyen/ekleyen metodu bildiriyorum
        int GetReminderHoursBeforeDefault(); // Varsayılan hatırlatma saatini dönen yardımcı metodu bildiriyorum
        int GetTwoHourAlertBeforeDefault(); // Varsayılan 2 saat uyarı süresini dönen yardımcı metodu bildiriyorum
    }
}

