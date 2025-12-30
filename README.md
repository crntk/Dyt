# ?? Dyt - Diyetisyen Web Sitesi

Modern ve kullanýcý dostu bir diyetisyen web sitesi projesi. ASP.NET Core MVC ile geliþtirilmiþ, randevu yönetimi, blog ve tarif paylaþýmý özellikleri içeren profesyonel bir platform.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat&logo=csharp)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=flat&logo=bootstrap)
![License](https://img.shields.io/badge/License-MIT-green?style=flat)

## ?? Ýçindekiler

- [Özellikler](#-özellikler)
- [Teknolojiler](#-teknolojiler)
- [Kurulum](#-kurulum)
- [Kullaným](#-kullaným)
- [Proje Yapýsý](#-proje-yapýsý)
- [Ekran Görüntüleri](#-ekran-görüntüleri)
- [Katkýda Bulunma](#-katkýda-bulunma)
- [Lisans](#-lisans)

## ? Özellikler

### ?? Ana Özellikler

- **?? Randevu Yönetimi**
  - Online randevu oluþturma
  - Çalýþma saatleri yönetimi
  - Randevu onay/iptal sistemi
  - E-posta bildirimleri

- **?? Blog Sistemi**
  - Blog yazýlarý yayýnlama
  - Kategori ve etiket yönetimi
  - Medya yükleme ve galeri
  - SEO dostu URL yapýsý (slug)

- **??? Tarif Paylaþýmý**
  - Saðlýklý tarif önerileri
  - Kalori ve besin deðerleri
  - Adým adým hazýrlýk talimatlarý
  - Popup detay görüntüleme

- **?? Kullanýcý Yönetimi**
  - Güvenli giriþ sistemi (ASP.NET Identity)
  - Rol tabanlý yetkilendirme
  - Profil yönetimi

- **?? Modern UI/UX**
  - Responsive tasarým
  - Dark/Light mod desteði
  - Smooth animasyonlar
  - Gradient efektleri
  - Scroll animasyonlarý

### ?? Admin Paneli

- Dashboard istatistikleri
- Randevu yönetimi
- Blog yazýsý yönetimi
- Çalýþma saatleri ayarlarý
- Kullanýcý yönetimi

## ??? Teknolojiler

### Backend
- **Framework:** ASP.NET Core 8.0 MVC
- **Dil:** C# 12.0
- **ORM:** Entity Framework Core
- **Veritabaný:** SQL Server / SQLite
- **Authentication:** ASP.NET Identity
- **Validation:** FluentValidation

### Frontend
- **UI Framework:** Bootstrap 5.3
- **CSS:** Custom CSS3 + Animations
- **JavaScript:** Vanilla JS + jQuery
- **Icons:** Font Awesome / Emoji
- **Responsive:** Mobile-first approach

### Proje Mimarisi
```
?? Dyt
??? ?? Dyt.Web (Presentation Layer)
??? ?? Dyt.Data (Data Access Layer)
??? ?? Dyt.Business (Business Logic Layer)
??? ?? Dyt.Contracts (DTOs & Interfaces)
```

## ?? Kurulum

### Gereksinimler

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) veya SQLite
- [Visual Studio 2022](https://visualstudio.microsoft.com/) veya [VS Code](https://code.visualstudio.com/)

### Adým Adým Kurulum

1. **Projeyi klonlayýn**
```bash
git clone https://github.com/crntk/Dyt.git
cd Dyt
```

2. **Baðýmlýlýklarý yükleyin**
```bash
dotnet restore
```

3. **Connection String ayarlayýn**

`Dyt.Web/appsettings.json` dosyasýný düzenleyin:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=DytDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

4. **Veritabanýný oluþturun**
```bash
cd Dyt.Data
dotnet ef database update
```

5. **Projeyi çalýþtýrýn**
```bash
cd ../Dyt.Web
dotnet run
```

6. **Tarayýcýda açýn**
```
https://localhost:5001
```

### ?? Varsayýlan Admin Hesabý

Ýlk çalýþtýrmada otomatik oluþturulur:
- **Email:** admin@diyetisyen.com
- **Þifre:** Admin123!

## ?? Kullaným

### Ziyaretçi Ýþlemleri

1. **Ana Sayfa:** Diyetisyenin profili, hizmetler ve son paylaþýmlarý görüntüleyin
2. **Randevu Al:** Online randevu oluþturun
3. **Blog:** Beslenme ile ilgili makaleleri okuyun
4. **Tarifler:** Saðlýklý tarif önerilerini keþfedin
5. **Ýletiþim:** Mesaj gönderin

### Admin Ýþlemleri

1. **Giriþ:** `/Account/Login` adresinden admin giriþi yapýn
2. **Dashboard:** `/Admin/Dashboard` - Genel istatistikleri görüntüleyin
3. **Randevular:** `/Admin/Appointments` - Randevularý yönetin
4. **Blog:** `/Admin/BlogAdmin` - Blog yazýlarýný düzenleyin
5. **Çalýþma Saatleri:** `/Admin/WorkingHours` - Müsait saatleri ayarlayýn

## ?? Proje Yapýsý

```
Dyt/
??? ?? Dyt.Web/      # Web Uygulamasý
?   ??? ?? Areas/
?   ?   ??? ?? Admin/   # Admin Panel
?   ?     ??? Controllers/
?   ? ??? Views/
?   ??? ?? Controllers/            # MVC Controllers
?   ??? ?? Views/      # Razor Views
?   ?   ??? Home/
?   ?   ??? Blog/
?   ?   ??? Appointment/
?   ?   ??? Contact/
?   ?   ??? Shared/
?   ??? ?? wwwroot/      # Static Files
?   ?   ??? css/
?   ?   ??? js/
?   ?   ??? images/
?   ?   ??? lib/
?   ??? ?? ViewComponents/      # View Components
?   ??? ?? Infrastructure/         # Seeds & Configs
?
??? ?? Dyt.Data/           # Data Layer
?   ??? ?? Entities/   # Domain Models
?   ?   ??? Appointment/
? ?   ??? Content/
?   ?   ??? Identity/
?   ??? ?? Configurations/         # EF Configurations
?   ??? ApplicationDbContext.cs
?
??? ?? Dyt.Business/         # Business Layer
?   ??? ?? Services/         # Business Services
?   ??? ?? Validators/          # FluentValidation
?   ??? ?? Mapping/                # AutoMapper
?
??? ?? Dyt.Contracts/              # Contracts
    ??? ?? DTOs/        # Data Transfer Objects
    ??? ?? Interfaces/         # Service Interfaces
```

## ?? Ekran Görüntüleri

### Ana Sayfa
Modern ve kullanýcý dostu arayüz, gradient efektler ve animasyonlar.

### Admin Dashboard
Randevu ve istatistikleri tek bir panelden yönetin.

### Blog Sayfasý
SEO dostu blog sistemi, kategori ve etiket filtreleme.

### Randevu Formu
Kullanýcý dostu randevu oluþturma formu, müsaitlik kontrolü.

## ?? Katkýda Bulunma

Katkýlarýnýzý bekliyoruz! Lütfen þu adýmlarý izleyin:

1. Bu projeyi fork edin
2. Yeni bir branch oluþturun (`git checkout -b feature/amazing-feature`)
3. Deðiþikliklerinizi commit edin (`git commit -m 'feat: Add amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request açýn

### Commit Kurallarý

- `feat:` Yeni özellik
- `fix:` Hata düzeltme
- `docs:` Dokümantasyon
- `style:` Kod formatý
- `refactor:` Kod refaktörü
- `test:` Test ekleme
- `chore:` Genel deðiþiklikler

## ?? Lisans

Bu proje [MIT License](LICENSE) ile lisanslanmýþtýr.

## ????? Geliþtirici

**Ceren TK**
- GitHub: [@crntk](https://github.com/crntk)

## ?? Teþekkürler

- [ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- [Bootstrap](https://getbootstrap.com)
- [Font Awesome](https://fontawesome.com)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)

## ?? Ýletiþim

Sorularýnýz için:
- ?? Email: [email protected]
- ?? Issues: [GitHub Issues](https://github.com/crntk/Dyt/issues)

---

? Bu projeyi beðendiyseniz yýldýz vermeyi unutmayýn!

**Not:** Bu proje eðitim ve portfolyo amaçlý geliþtirilmiþtir.
