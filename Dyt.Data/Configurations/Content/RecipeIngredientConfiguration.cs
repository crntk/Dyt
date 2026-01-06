using Dyt.Data.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dyt.Data.Configurations.Content
{
 /// <summary>
    /// RecipeIngredient varlýðýna ait tablo konfigürasyonlarýný içerir.
  /// </summary>
  public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
    {
        public void Configure(EntityTypeBuilder<RecipeIngredient> b)
        {
     b.Property(x => x.Name).IsRequired().HasMaxLength(200);
   b.Property(x => x.DisplayOrder).HasDefaultValue(0);
     b.Property(x => x.RowVersion).IsRowVersion();

            b.HasIndex(x => new { x.RecipeId, x.DisplayOrder });
        }
    }
}
