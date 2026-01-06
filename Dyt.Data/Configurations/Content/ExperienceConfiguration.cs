using Dyt.Data.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dyt.Data.Configurations.Content
{
 /// <summary>
    /// Experience entity configuration
    /// </summary>
    public class ExperienceConfiguration : IEntityTypeConfiguration<Experience>
    {
        public void Configure(EntityTypeBuilder<Experience> builder)
        {
   builder.ToTable("Experiences");

  builder.HasKey(x => x.Id);

         builder.Property(x => x.Position)
         .IsRequired()
       .HasMaxLength(200);

 builder.Property(x => x.Institution)
 .IsRequired()
     .HasMaxLength(200);

            builder.Property(x => x.Description)
             .HasMaxLength(1000);

        builder.Property(x => x.StartDate)
         .IsRequired();

            builder.Property(x => x.EndDate);

          builder.Property(x => x.IsCurrent)
          .IsRequired();

   builder.Property(x => x.Type)
      .IsRequired()
  .HasMaxLength(50);

     builder.Property(x => x.DisplayOrder)
.IsRequired();

  builder.Property(x => x.IsActive)
    .IsRequired();

      builder.Property(x => x.CreatedAt)
        .IsRequired();

     builder.Property(x => x.UpdatedAt);

// Index
 builder.HasIndex(x => new { x.IsActive, x.DisplayOrder });
}
    }
}
