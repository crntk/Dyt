# Email Yapýlandýrmasý

Bu proje, blog yazýlarý yayýnlandýðýnda bülten abonelerine otomatik email gönderimi yapmaktadýr.

## Gmail SMTP ile Kurulum

### 1. Gmail Uygulama Þifresi Oluþturma

Gmail hesabýnýzla SMTP kullanmak için **Uygulama Þifresi** oluþturmanýz gerekir:

1. Google Hesabýnýza gidin: https://myaccount.google.com/
2. **Güvenlik** sekmesine týklayýn
3. **2 Adýmlý Doðrulama**'yý aktif edin (eðer aktif deðilse)
4. **Uygulama þifreleri** bölümüne gidin
5. "Uygulama seç" menüsünden "Mail" seçin
6. "Cihaz seç" menüsünden "Windows Bilgisayar" veya "Diðer" seçin
7. "Oluþtur" butonuna týklayýn
8. Gösterilen **16 haneli þifreyi** kopyalayýn

### 2. appsettings.json Güncelleme

`Dyt.Web/appsettings.json` dosyasýný açýn ve Email bölümünü güncelleyin:

```json
"Email": {
  "SmtpHost": "smtp.gmail.com",
  "SmtpPort": 587,
  "SmtpUsername": "sizin-email@gmail.com",           // Gmail adresiniz
  "SmtpPassword": "abcd efgh ijkl mnop",             // 16 haneli uygulama þifreniz (boþluklarla)
  "FromEmail": "sizin-email@gmail.com",      // Gmail adresiniz
  "FromName": "Diyetisyen Riyaza Tair",        // Gönderen adý
  "EnableSsl": true,
  "EnableRealSend": true // Production'da true yapýn
}
```

### 3. Test Etme

#### Development Ortamýnda Test (Sadece Log):
```json
// appsettings.Development.json
{
  "Email": {
    "EnableRealSend": false  // Email gönderilmez, sadece log'a yazar
  }
}
```

#### Production/Test Ýçin (Gerçek Gönderim):
```json
// appsettings.json
{
  "Email": {
    "EnableRealSend": true   // Gerçekten email gönderir
  }
}
```

### 4. Bülten Aboneliði Ekleme

Test etmek için önce bir abone ekleyin:

1. Web sitesini çalýþtýrýn
2. Footer'daki "Bülten" bölümünden email adresinizi girin
3. Veritabanýnda `NewsletterSubscribers` tablosunu kontrol edin

### 5. Blog Yazýsý Yayýnlama ve Email Gönderimi

1. Admin paneline giriþ yapýn: `/diyetisyen`
2. "Blog Paylaþ" sayfasýna gidin
3. Bir fotoðraf veya makale paylaþýn
4. Email otomatik olarak tüm abonelere gönderilecektir

### 6. Loglarý Kontrol Etme

Email gönderim durumunu kontrol etmek için:

- **Visual Studio Output** penceresini açýn
- `[Email]` ile baþlayan loglarý arayýn
- Baþarýlý: `[Email] SENT - To=...`
- Baþarýsýz: `[Email] FAILED - To=...`

## Diðer Email Servisleri

### Outlook/Hotmail SMTP
```json
{
  "SmtpHost": "smtp-mail.outlook.com",
  "SmtpPort": 587,
  "SmtpUsername": "sizin-email@outlook.com",
  "SmtpPassword": "parolaniz",
  "FromEmail": "sizin-email@outlook.com"
}
```

### Özel SMTP Sunucusu
Kendi SMTP sunucunuzu kullanýyorsanýz:
```json
{
  "SmtpHost": "mail.siteadi.com",
  "SmtpPort": 587,
  "SmtpUsername": "info@siteadi.com",
  "SmtpPassword": "parolaniz",
  "FromEmail": "info@siteadi.com"
}
```

## Güvenlik Notlarý

?? **ÖNEMLÝ**: 
- `appsettings.json` dosyasýný **asla** Git'e commit etmeyin
- Üretim ortamýnda þifreleri **User Secrets** veya **Azure Key Vault** kullanarak saklayýn
- `.gitignore` dosyasýnda `appsettings.json` olduðundan emin olun

### User Secrets Kullanýmý (Önerilen)

Development ortamýnda User Secrets kullanmak daha güvenlidir:

```bash
# User Secrets baþlat
dotnet user-secrets init --project Dyt.Web

# Email ayarlarýný ekle
dotnet user-secrets set "Email:SmtpUsername" "sizin-email@gmail.com" --project Dyt.Web
dotnet user-secrets set "Email:SmtpPassword" "abcd efgh ijkl mnop" --project Dyt.Web
dotnet user-secrets set "Email:FromEmail" "sizin-email@gmail.com" --project Dyt.Web
dotnet user-secrets set "Email:EnableRealSend" "true" --project Dyt.Web
```

## Sorun Giderme

### "Authentication failed" hatasý
- Gmail uygulama þifresinin doðru olduðundan emin olun
- 2 Adýmlý Doðrulama'nýn aktif olduðunu kontrol edin

### "Unable to connect to remote server" hatasý
- Ýnternet baðlantýnýzý kontrol edin
- Firewall/antivirus ayarlarýný kontrol edin
- SMTP portunu kontrol edin (587 veya 465)

### Email gönderilmiyor ama hata yok
- `EnableRealSend` ayarýnýn `true` olduðundan emin olun
- Log'larda `[Email] SIMULATED` yazýyorsa, gerçek gönderim kapalýdýr

### Spam klasörüne düþüyor
- SPF, DKIM, DMARC kayýtlarýný domain'inize ekleyin
- "Kimden" adresinin doðru olduðundan emin olun
- Ýçeriðin spam tetikleyici kelimeler içermediðini kontrol edin

## Performans

- Email gönderimi **arka planda** (`Task.Run`) çalýþýr
- Blog paylaþýmý hemen tamamlanýr, emailler eþzamansýz gönderilir
- Çok sayýda abone varsa gönderim biraz zaman alabilir
- Gönderim durumu log'lara yazýlýr

## Ýletiþim

Sorun yaþarsanýz loglara bakýn veya projeyi geliþtiren kiþiye danýþýn.
