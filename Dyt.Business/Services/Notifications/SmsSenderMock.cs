using Dyt.Business.Interfaces.Notifications; // ISmsSender arayüzünü kullanmak için ekliyorum
using Dyt.Business.Options;                  // SmsOptions yapılandırmasına erişmek için ekliyorum
using Microsoft.Extensions.Logging;          // Günlükleme altyapısı için ekliyorum
using Microsoft.Extensions.Options;          // IOptions desenini kullanmak için ekliyorum

namespace Dyt.Business.Services.Notifications // Bildirim servislerinin yer aldığı ad alanını tanımlıyorum
{
    /// <summary>
    /// Geliştirme ve test ortamları için SMS gönderimini taklit eden sahte gönderici.
    /// Canlıda gerçek sağlayıcı implementasyonu ile değiştirilecektir.
    /// </summary>
    public class SmsSenderMock : ISmsSender // ISmsSender sözleşmesini uygulayan sınıfı tanımlıyorum
    {
        private readonly ILogger<SmsSenderMock> _log; // Günlükleme yapmak için logger alanı tanımlıyorum
        private readonly SmsOptions _opt;             // Sms ile ilgili ayarları tutmak için alan tanımlıyorum

        /// <summary>
        /// Logger ve konfigürasyon seçeneklerini alarak sınıfın örneğini oluşturur.
        /// </summary>
        public SmsSenderMock(ILogger<SmsSenderMock> log, IOptions<SmsOptions> opt) // Kurucu metotta bağımlılıkları alıyorum
        {
            _log = log;            // Verilen logger örneğini alan değişkenine atıyorum
            _opt = opt.Value;      // IOptions içinden gerçek SmsOptions değerini alıp alan değişkenine atıyorum
        }

        /// <summary>
        /// E.164 formatındaki bir numaraya SMS gönderimini taklit eder; gerçek gönderim yapmaz.
        /// </summary>
        /// <param name="phoneE164">Alıcının E.164 formatındaki telefon numarası.</param>
        /// <param name="message">Gönderilecek SMS metni.</param>
        /// <param name="ct">İptal belirteci.</param>
        /// <returns>Başarı durumu, sağlayıcı mesaj kimliği ve hata mesajını içeren tuple döner.</returns>
        public Task<(bool Success, string? ProviderMessageId, string? Error)> SendAsync(string phoneE164, string message, CancellationToken ct = default) // Arayüzdeki imzayla birebir aynı dönüş tipini kullanıyorum
        {
            var id = Guid.NewGuid().ToString("N"); // Sağlayıcı mesaj kimliği gibi davranması için rasgele bir kimlik üretiyorum

            _log.LogInformation( // Gönderim işlemini simüle ederek log'a yazıyorum
                "SMS MOCK gönderimi: Alıcı={Phone} | Başlık={Sender} | MesajId={Id} | İçerik={Message}", // Log formatını belirtiyorum
                phoneE164,    // Alıcı numarayı parametre olarak veriyorum
                _opt.SenderName, // Gönderen adını konfigürasyondan okuyup parametre olarak veriyorum
                id,           // Ürettiğim sahte mesaj kimliğini parametre olarak veriyorum
                message);     // Mesaj içeriğini parametre olarak veriyorum

            // İstenen tuple tipini açıkça belirterek döndürüyorum; nullability uyarılarını önlüyorum
            return Task.FromResult<(bool Success, string? ProviderMessageId, string? Error)>((true, id, null)); // Başarıyı true, mesaj kimliğini ürettiğim id, hatayı null olarak döndürüyorum
        }
    }
}
