using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Business.Interfaces.Notifications // Bildirim/SMS servisleri için ad alanını tanımlıyorum
{
    /// <summary>
    /// SMS gönderimi için soyutlama. Geliştirme sırasında mock, canlıda gerçek sağlayıcı ile değiştirilecektir.
    /// </summary>
    public interface ISmsSender // SMS gönderim servisi arayüzü
    {
        Task<(bool Success, string? ProviderMessageId, string? Error)> SendAsync(string phoneE164, string message, CancellationToken ct = default); // E.164 formatında numaraya SMS gönderimi metodunu bildiriyorum
    }
}

