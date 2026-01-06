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
    /// Deneyim/Özgeçmiþ yönetim servisi
  /// </summary>
    public class ExperienceService : IExperienceService
    {
        private readonly AppDbContext _db;

   public ExperienceService(AppDbContext db)
        {
            _db = db;
   }

     public async Task<List<ExperienceDto>> GetAllAsync(CancellationToken ct = default)
   {
 var experiences = await _db.Experiences
     .Where(x => x.IsActive)
     .OrderBy(x => x.DisplayOrder)
          .ToListAsync(ct);

   return experiences.Select(e => new ExperienceDto
        {
          Id = e.Id,
   Position = e.Position,
    Institution = e.Institution,
         Description = e.Description,
      StartDate = e.StartDate,
         EndDate = e.EndDate,
           IsCurrent = e.IsCurrent,
             Type = e.Type,
DisplayOrder = e.DisplayOrder,
 IsActive = e.IsActive,
      CreatedAt = e.CreatedAt,
  UpdatedAt = e.UpdatedAt
            }).ToList();
}

public async Task<ExperienceDto?> GetByIdAsync(int id, CancellationToken ct = default)
     {
      var exp = await _db.Experiences.FindAsync(new object[] { id }, ct);
        if (exp == null || !exp.IsActive) return null;

      return new ExperienceDto
      {
          Id = exp.Id,
         Position = exp.Position,
                Institution = exp.Institution,
 Description = exp.Description,
             StartDate = exp.StartDate,
  EndDate = exp.EndDate,
          IsCurrent = exp.IsCurrent,
Type = exp.Type,
       DisplayOrder = exp.DisplayOrder,
     IsActive = exp.IsActive,
   CreatedAt = exp.CreatedAt,
UpdatedAt = exp.UpdatedAt
    };
    }

    public async Task<ExperienceDto> CreateAsync(ExperienceRequest request, CancellationToken ct = default)
        {
    var exp = new Experience
       {
   Position = request.Position,
      Institution = request.Institution,
    Description = request.Description,
  StartDate = request.StartDate,
         EndDate = request.EndDate,
 IsCurrent = request.IsCurrent,
     Type = request.Type,
 DisplayOrder = request.DisplayOrder,
      IsActive = request.IsActive,
     CreatedAt = DateTime.UtcNow
    };

  _db.Experiences.Add(exp);
      await _db.SaveChangesAsync(ct);

        return new ExperienceDto
{
    Id = exp.Id,
   Position = exp.Position,
   Institution = exp.Institution,
        Description = exp.Description,
    StartDate = exp.StartDate,
   EndDate = exp.EndDate,
       IsCurrent = exp.IsCurrent,
   Type = exp.Type,
     DisplayOrder = exp.DisplayOrder,
    IsActive = exp.IsActive,
      CreatedAt = exp.CreatedAt,
      UpdatedAt = exp.UpdatedAt
     };
 }

        public async Task<ExperienceDto?> UpdateAsync(int id, ExperienceRequest request, CancellationToken ct = default)
   {
var exp = await _db.Experiences.FindAsync(new object[] { id }, ct);
     if (exp == null || !exp.IsActive) return null;

            exp.Position = request.Position;
        exp.Institution = request.Institution;
          exp.Description = request.Description;
  exp.StartDate = request.StartDate;
      exp.EndDate = request.EndDate;
         exp.IsCurrent = request.IsCurrent;
     exp.Type = request.Type;
       exp.DisplayOrder = request.DisplayOrder;
      exp.IsActive = request.IsActive;
     exp.UpdatedAt = DateTime.UtcNow;

     await _db.SaveChangesAsync(ct);

            return new ExperienceDto
      {
           Id = exp.Id,
     Position = exp.Position,
      Institution = exp.Institution,
    Description = exp.Description,
  StartDate = exp.StartDate,
                EndDate = exp.EndDate,
 IsCurrent = exp.IsCurrent,
    Type = exp.Type,
         DisplayOrder = exp.DisplayOrder,
       IsActive = exp.IsActive,
            CreatedAt = exp.CreatedAt,
     UpdatedAt = exp.UpdatedAt
      };
 }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
  {
  var exp = await _db.Experiences.FindAsync(new object[] { id }, ct);
       if (exp == null) return false;

 exp.IsActive = false;
   exp.UpdatedAt = DateTime.UtcNow;

    await _db.SaveChangesAsync(ct);
    return true;
        }
}
}
