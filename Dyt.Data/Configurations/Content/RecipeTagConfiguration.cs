using Dyt.Data.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dyt.Data.Configurations.Content
{
  /// <summary>
    /// RecipeTag çoka-çok iliþki tablosu konfigürasyonu.
    /// </summary>
 public class RecipeTagConfiguration : IEntityTypeConfiguration<RecipeTag>
    {
   public void Configure(EntityTypeBuilder<RecipeTag> b)
  {
    b.HasKey(x => new { x.RecipeId, x.TagId });

  b.HasOne(x => x.Recipe)
 .WithMany(x => x.RecipeTags)
     .HasForeignKey(x => x.RecipeId)
        .OnDelete(DeleteBehavior.Cascade);

  b.HasOne(x => x.Tag)
    .WithMany()
     .HasForeignKey(x => x.TagId)
           .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
