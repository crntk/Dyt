using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Business.Interfaces.Notifications // Bildirim şablon servisleri için ad alanını tanımlıyorum
{
    /// <summary>
    /// SMS bildirim metinlerini şablon + değişkenlerle oluşturan servis.
    /// </summary>
    public interface INotificationTemplateService // Şablon işleme arayüzü
    {
        string RenderReminder(string clientName, DateOnly date, TimeOnly start, string confirmYesUrl, string confirmNoUrl); // Hatırlatma SMS metnini üreten metodu bildiriyorum
        string RenderTwoHourNoResponseAlert(string clientName, DateOnly date, TimeOnly start); // 2 saat kala yanıt yok uyarı metnini üreten metodu bildiriyorum
    }
}
