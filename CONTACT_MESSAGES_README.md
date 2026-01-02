# Ýletiþim Mesajlarý Özelliði - Kurulum Talimatlarý

Bu özellik, siteden gelen iletiþim formlarý mesajlarýnýn veritabanýna kaydedilmesini ve admin panelinden yönetilmesini saðlar.

## Yapýlan Deðiþiklikler

### 1. Yeni Dosyalar

- **Dyt.Data\Entities\Content\ContactMessage.cs** - Ýletiþim mesajý entity'si
- **Dyt.Data\Configurations\Content\ContactMessageConfiguration.cs** - Entity configuration
- **Dyt.Web\Areas\Admin\Controllers\ContactMessagesController.cs** - Admin controller
- **Dyt.Web\Areas\Admin\Views\ContactMessages\Index.cshtml** - Mesaj listesi view
- **Dyt.Web\Areas\Admin\Views\ContactMessages\Details.cshtml** - Mesaj detay view

### 2. Güncellenen Dosyalar

- **Dyt.Data\Context\AppDbContext.cs** - ContactMessages DbSet eklendi
- **Dyt.Web\Controllers\ContactController.cs** - Mesajlarý veritabanýna kaydetme özelliði eklendi
- **Dyt.Web\Views\Shared\_Layout.cshtml** - Admin menüsüne "Ýletiþim Mesajlarý" linki eklendi

## Veritabaný Migration

Aþaðýdaki komutlarý çalýþtýrarak migration'ý oluþturup veritabanýný güncelleyin:

```powershell
# Workspace ana dizinine gidin
cd "C:\Users\ceren\OneDrive\Masaüstü\Dyt\Dyt\Dyt"

# Migration oluþtur
dotnet ef migrations add AddContactMessages --project Dyt.Data --startup-project Dyt.Web

# Veritabanýný güncelle
dotnet ef database update --project Dyt.Data --startup-project Dyt.Web
```

## Özellikler

### Ýletiþim Formu (Public)
- Siteden gelen mesajlar veritabanýna kaydedilir
- E-posta bildirimi gönderilmeye devam eder
- KVKK onayý kaydedilir

### Admin Paneli
- **Mesaj Listesi**: Tüm mesajlarý görüntüleme
- **Filtreler**: Okunmuþ/Okunmamýþ, Yanýtlanmýþ/Yanýtlanmamýþ
- **Mesaj Detayý**: Mesaj içeriðini görüntüleme
- **Hýzlý Eylemler**:
  - E-posta gönder
  - Telefon ara
  - WhatsApp mesaj gönder
  - Okundu/Yanýtlandý olarak iþaretle
  - Admin notu ekleme
  - Mesaj silme (soft delete)

## Kullaným

1. Admin paneline giriþ yapýn
2. Üst menüden "Ýletiþim Mesajlarý"na týklayýn
3. Mesaj listesini görüntüleyin
4. Bir mesaja týklayarak detaylarý görün
5. Hýzlý eylemler ile mesajý yönetin

## Güvenlik

- Sadece Admin rolündeki kullanýcýlar iletiþim mesajlarýný görebilir
- Tüm admin aksiyonlar için CSRF korumasý aktif
- Soft delete kullanýlarak mesajlar kalýcý olarak silinmez

## Veritabaný Yapýsý

### ContactMessages Tablosu

| Kolon | Tip | Açýklama |
|-------|-----|----------|
| Id | int | Birincil anahtar |
| Name | nvarchar(80) | Gönderen adý |
| Email | nvarchar(160) | E-posta adresi |
| Phone | nvarchar(20) | Telefon numarasý |
| Subject | nvarchar(120) | Mesaj konusu |
| Message | nvarchar(2000) | Mesaj içeriði |
| KvkkConsent | bit | KVKK onayý |
| IsRead | bit | Okundu mu? |
| IsReplied | bit | Yanýtlandý mý? |
| AdminNote | nvarchar(1000) | Admin notu |
| ReadAt | datetime2 | Okunma tarihi |
| RepliedAt | datetime2 | Yanýtlanma tarihi |
| CreatedAtUtc | datetime2 | Oluþturulma tarihi |
| UpdatedAtUtc | datetime2 | Güncellenme tarihi |
| IsDeleted | bit | Soft delete |
| DeletedAtUtc | datetime2 | Silinme tarihi |
| RowVersion | rowversion | Eþzamanlýlýk kontrolü |

### Ýndeksler

- Email
- IsRead
- CreatedAtUtc
