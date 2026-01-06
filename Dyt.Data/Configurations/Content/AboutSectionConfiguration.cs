using Dyt.Data.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dyt.Data.Configurations.Content
{
    /// <summary>
    /// AboutSection entity configuration
    /// </summary>
    public class AboutSectionConfiguration : IEntityTypeConfiguration<AboutSection>
    {
        public void Configure(EntityTypeBuilder<AboutSection> builder)
        {
            builder.ToTable("AboutSections");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.ContentMarkdown)
                 .IsRequired()
                .HasMaxLength(5000);

            builder.Property(x => x.IsPublished)
                .IsRequired();

            // Audit fields
            builder.Property(x => x.CreatedAtUtc).IsRequired();
            builder.Property(x => x.UpdatedAtUtc);

            // Soft delete
            builder.Property(x => x.IsDeleted).IsRequired();
            builder.Property(x => x.DeletedAtUtc);
        }
    }
}
