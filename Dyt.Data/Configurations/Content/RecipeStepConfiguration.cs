using Dyt.Data.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dyt.Data.Configurations.Content
{
    /// <summary>
 /// RecipeStep varlýðýna ait tablo konfigürasyonlarýný içerir.
    /// </summary>
    public class RecipeStepConfiguration : IEntityTypeConfiguration<RecipeStep>
    {
 public void Configure(EntityTypeBuilder<RecipeStep> b)
        {
   b.Property(x => x.Description).IsRequired().HasMaxLength(1000);
    b.Property(x => x.StepNumber).IsRequired();
       b.Property(x => x.RowVersion).IsRowVersion();

        b.HasIndex(x => new { x.RecipeId, x.StepNumber });
        }
  }
}
