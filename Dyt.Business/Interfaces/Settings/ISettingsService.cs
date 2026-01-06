using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Contracts.Settings;
using Dyt.Contracts.Settings.Responses; // Ayar DTO'larýný kullanmak için ekliyorum

namespace Dyt.Business.Interfaces.Settings // Ayarlar servisleri için ad alanýný tanýmlýyorum
{
    /// <summary>
    /// Uygulama genel ayarlarýnýn okunmasý ve güncellenmesi için servis sözleþmesi.
    /// </summary>
    public interface ISettingsService // Ayar servisi arayüzü
    {
        Task<SystemSettingDto?> GetAsync(string key, CancellationToken ct = default); // Anahtara göre ayar deðeri getiren metodu bildiriyorum
        Task<bool> SetAsync(string key, string value, CancellationToken ct = default); // Anahtara göre ayar deðerini güncelleyen/ekleyen metodu bildiriyorum
        int GetReminderHoursBeforeDefault(); // Varsayýlan hatýrlatma saatini dönen yardýmcý metodu bildiriyorum
        int GetTwoHourAlertBeforeDefault(); // Varsayýlan 2 saat uyarý süresini dönen yardýmcý metodu bildiriyorum
    }
}

