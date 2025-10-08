using System.ComponentModel.DataAnnotations; // Form doğrulama öznitelikleri için ekliyorum

namespace Dyt.Web.ViewModels // Web katmanına özgü ViewModel'ler için ad alanı belirliyorum
{
    /// <summary>
    /// Ziyaretçi tarafında randevu oluşturma formu için ViewModel.
    /// Bu model sadece UI doğrulaması içindir; Business katmanına DTO ile gideceğiz.
    /// </summary>
    public class AppointmentCreateVm // Form verilerini tutacak sınıfı tanımlıyorum
    {
        [Required] // Tarih zorunlu olsun istiyorum
        public DateOnly? Date { get; set; } // Kullanıcının seçtiği tarihi tutuyorum (nullable ki boşken modelstate hata üretebilsin)

        [Required] // Başlangıç saati zorunlu
        public TimeOnly? StartTime { get; set; } // Seçilen slotun başlangıcını tutuyorum

        [Required, StringLength(80)] // Ad zorunlu ve sınırlı
        public string ClientName { get; set; } = string.Empty; // Danışan adını tutuyorum

        [Required, Phone] // Telefon zorunlu ve format kontrolü olsun
        public string ClientPhone { get; set; } = string.Empty; // Danışan telefonunu tutuyorum

        [EmailAddress] // E-posta opsiyonel ama girilirse formatı doğru olmalı
        public string? ClientEmail { get; set; } // Danışan e-postasını tutuyorum

        [StringLength(250)] // Not alanına üst sınır koyuyorum
        public string? Note { get; set; } // Opsiyonel not bilgisini tutuyorum

        [Required(ErrorMessage = "Lütfen görüşme türünü seçiniz (Online / Yüzyüze)")] // Görüşme türü zorunlu
        [RegularExpression("^(Online|Yüzyüze)$", ErrorMessage = "Geçerli bir görüşme türü seçiniz.")]
        public string Channel { get; set; } = string.Empty; // Online veya Yüzyüze
    }
}
