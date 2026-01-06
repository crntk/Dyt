using Dyt.Business.Interfaces.About;
using Dyt.Contracts.About.Requests;
using Dyt.Contracts.About.Responses;
using Dyt.Data.Context;
using Dyt.Data.Entities.Content;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dyt.Business.Services.About
{
    /// <summary>
    /// Sertifika/Baþarý yönetim servisi
   /// </summary>
    public class CertificateService : ICertificateService
    {
        private readonly AppDbContext _db;

      public CertificateService(AppDbContext db)
        {
     _db = db;
        }

       public async Task<List<CertificateDto>> GetAllPublishedAsync(CancellationToken ct = default)
 {
      var certificates = await _db.Certificates
           .Where(x => !x.IsDeleted && x.IsPublished)
      .OrderBy(x => x.DisplayOrder)
           .ToListAsync(ct);

     return certificates.Select(c => new CertificateDto
    {
        Id = c.Id,
  Title = c.Title,
 Issuer = c.Issuer,
      IssueDate = c.IssueDate,
    Description = c.Description,
     ImageUrl = c.ImageUrl,
VerificationUrl = c.VerificationUrl,
      DisplayOrder = c.DisplayOrder,
 IsPublished = c.IsPublished,
    CreatedAtUtc = c.CreatedAtUtc,
     UpdatedAtUtc = c.UpdatedAtUtc
     }).ToList();
  }

        public async Task<CertificateDto?> GetByIdAsync(int id, CancellationToken ct = default)
{
    var cert = await _db.Certificates.FindAsync(new object[] { id }, ct);
   if (cert == null || cert.IsDeleted) return null;

    return new CertificateDto
    {
                Id = cert.Id,
         Title = cert.Title,
       Issuer = cert.Issuer,
    IssueDate = cert.IssueDate,
             Description = cert.Description,
     ImageUrl = cert.ImageUrl,
      VerificationUrl = cert.VerificationUrl,
       DisplayOrder = cert.DisplayOrder,
    IsPublished = cert.IsPublished,
       CreatedAtUtc = cert.CreatedAtUtc,
          UpdatedAtUtc = cert.UpdatedAtUtc
       };
   }

   public async Task<CertificateDto> CreateAsync(CertificateRequest request, CancellationToken ct = default)
     {
    var cert = new Certificate
       {
     Title = request.Title,
     Issuer = request.Issuer,
        IssueDate = request.IssueDate,
 Description = request.Description,
     ImageUrl = request.ImageUrl,
    VerificationUrl = request.VerificationUrl,
 DisplayOrder = request.DisplayOrder,
 IsPublished = request.IsPublished,
    CreatedAtUtc = DateTime.UtcNow
       };

  _db.Certificates.Add(cert);
      await _db.SaveChangesAsync(ct);

       return new CertificateDto
    {
     Id = cert.Id,
      Title = cert.Title,
     Issuer = cert.Issuer,
      IssueDate = cert.IssueDate,
    Description = cert.Description,
    ImageUrl = cert.ImageUrl,
 VerificationUrl = cert.VerificationUrl,
      DisplayOrder = cert.DisplayOrder,
       IsPublished = cert.IsPublished,
     CreatedAtUtc = cert.CreatedAtUtc,
   UpdatedAtUtc = cert.UpdatedAtUtc
};
 }

  public async Task<CertificateDto?> UpdateAsync(int id, CertificateRequest request, CancellationToken ct = default)
 {
    var cert = await _db.Certificates.FindAsync(new object[] { id }, ct);
       if (cert == null || cert.IsDeleted) return null;

  cert.Title = request.Title;
  cert.Issuer = request.Issuer;
cert.IssueDate = request.IssueDate;
     cert.Description = request.Description;
     cert.ImageUrl = request.ImageUrl;
       cert.VerificationUrl = request.VerificationUrl;
   cert.DisplayOrder = request.DisplayOrder;
       cert.IsPublished = request.IsPublished;
      cert.UpdatedAtUtc = DateTime.UtcNow;

await _db.SaveChangesAsync(ct);

       return new CertificateDto
       {
       Id = cert.Id,
   Title = cert.Title,
   Issuer = cert.Issuer,
           IssueDate = cert.IssueDate,
  Description = cert.Description,
       ImageUrl = cert.ImageUrl,
   VerificationUrl = cert.VerificationUrl,
    DisplayOrder = cert.DisplayOrder,
  IsPublished = cert.IsPublished,
      CreatedAtUtc = cert.CreatedAtUtc,
     UpdatedAtUtc = cert.UpdatedAtUtc
   };
        }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
    var cert = await _db.Certificates.FindAsync(new object[] { id }, ct);
  if (cert == null || cert.IsDeleted) return false;

       cert.IsDeleted = true;
   cert.DeletedAtUtc = DateTime.UtcNow;

   await _db.SaveChangesAsync(ct);
   return true;
  }
    }
}
