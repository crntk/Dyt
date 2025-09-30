namespace Dyt.Contracts.Appointments.Requests // Randevu sorgu isteklerinin tutulduğu ad alanını tanımlıyorum
{
    /// <summary>
    /// Admin tarafında randevuları filtreleyip sayfalayarak listelemek için istek modeli.
    /// </summary>
    public class AppointmentQueryRequest // Sorgu parametrelerini taşıyan sınıfı tanımlıyorum
    {
        public int Page { get; set; } = 1; // Varsayılan sayfayı 1 olarak ayarlıyorum
        public int PageSize { get; set; } = 10; // Varsayılan sayfa boyutunu 10 olarak ayarlıyorum

        public DateOnly? DateFrom { get; set; } // Başlangıç tarihi filtresini tutuyorum (opsiyonel)
        public DateOnly? DateTo { get; set; } // Bitiş tarihi filtresini tutuyorum (opsiyonel)

        public string? Status { get; set; } // Randevu durum filtresini tutuyorum (Scheduled/Canceled/NoShow vb.)
        public string? ConfirmationState { get; set; } // Onay durumu filtresini tutuyorum (Yanıtlanmadı/Gelecek/Gelmeyecek)

        public string? Search { get; set; } // İsim/telefon metin arama filtresini tutuyorum (opsiyonel)
    }
}
