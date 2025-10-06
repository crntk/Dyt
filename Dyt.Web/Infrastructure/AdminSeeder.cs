using Dyt.Data.Context;                     // AppDbContext'e erişmek için ekliyorum
using Dyt.Data.Entities.Identity;           // AppUser, AppRole için ekliyorum
using Microsoft.AspNetCore.Identity;        // UserManager/RoleManager için ekliyorum
using Microsoft.EntityFrameworkCore;        // Veritabanı kontrolü için ekliyorum

namespace Dyt.Web.Infrastructure
{
    /// <summary>
    /// Uygulama ilk ayağa kalktığında tek seferlik admin kullanıcıyı üreten yardımcı.
    /// </summary>
    public static class AdminSeeder
    {
        /// <summary>
        /// Konfigürasyondaki Admin bölümünden bilgileri okuyup kullanıcı/rol oluşturur.
        /// </summary>
        public static async Task SeedAsync(IServiceProvider services) // DI sağlayıcısını alıyorum
        {
            using var scope = services.CreateScope(); // Scoped servisleri kullanmak için scope açıyorum
            var sp = scope.ServiceProvider; // Kısa isim

            var db = sp.GetRequiredService<AppDbContext>();              // DbContext'i alıyorum
            await db.Database.MigrateAsync();                            // Eksik migration varsa uyguluyorum

            var users = sp.GetRequiredService<UserManager<AppUser>>();   // UserManager alıyorum
            var roles = sp.GetRequiredService<RoleManager<AppRole>>();   // RoleManager alıyorum
            var cfg = sp.GetRequiredService<IConfiguration>().GetSection("Admin"); // Admin bölümünü okuyorum

            var email = cfg["Email"] ?? "admin@dyt.local";               // E-posta
            var pass = cfg["Password"] ?? "Admin!123";                   // Parola
            var name = cfg["FullName"] ?? "Diyetisyen Admin";            // İsim
            var phone = cfg["Phone"] ?? "+905551112233";                 // Telefon

            const string roleName = "Admin";                             // Tek rol adı

            if (!await roles.RoleExistsAsync(roleName))                   // Rol yoksa
                await roles.CreateAsync(new AppRole { Name = roleName }); // Rolü oluşturuyorum

            var user = await users.FindByEmailAsync(email);              // Kullanıcı var mı bakıyorum
            if (user == null)                                            // Yoksa
            {
                user = new AppUser                                     // Yeni kullanıcıyı hazırlıyorum
                {
                    UserName = email,                                  // Kullanıcı adı
                    Email = email,                                     // E-posta
                    FullName = name,                                   // Ad-soyad
                    PhoneNumber = phone,                               // Telefon
                    EmailConfirmed = true                              // E-posta doğrulandı işareti
                };

                var created = await users.CreateAsync(user, pass);       // Kullanıcıyı oluşturuyorum
                if (!created.Succeeded) throw new Exception(string.Join(";", created.Errors.Select(e => e.Description))); // Hata varsa fırlatıyorum
            }

            if (!await users.IsInRoleAsync(user, roleName))              // Rolü yoksa
                await users.AddToRoleAsync(user, roleName);              // Admin rolü veriyorum
        }
    }
}
