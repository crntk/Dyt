using Dyt.Data.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dyt.Data.Configurations.Content
{
    /// <summary>
    /// Recipe varlýðýna ait tablo konfigürasyonlarýný içerir.
    /// </summary>
 public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
    {
        public void Configure(EntityTypeBuilder<Recipe> b)
        {
     b.Property(x => x.Title).IsRequired().HasMaxLength(200);
       b.Property(x => x.Slug).IsRequired().HasMaxLength(220);
        b.HasIndex(x => x.Slug).IsUnique();
 b.Property(x => x.Summary).HasMaxLength(500);
    b.Property(x => x.ImageUrl).HasMaxLength(500);
        b.Property(x => x.PrepTime).HasMaxLength(50);
            b.Property(x => x.Calories).HasMaxLength(50);
 b.Property(x => x.Difficulty).HasMaxLength(20);
       b.Property(x => x.RowVersion).IsRowVersion();

       // Ýliþkiler
         b.HasMany(x => x.Ingredients)
    .WithOne(x => x.Recipe)
       .HasForeignKey(x => x.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

       b.HasMany(x => x.Steps)
     .WithOne(x => x.Recipe)
 .HasForeignKey(x => x.RecipeId)
  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
