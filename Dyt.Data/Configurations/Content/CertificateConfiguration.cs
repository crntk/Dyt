using Dyt.Data.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dyt.Data.Configurations.Content
{
    /// <summary>
    /// Certificate entity configuration
    /// </summary>
  public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
   {
        public void Configure(EntityTypeBuilder<Certificate> builder)
   {
          builder.ToTable("Certificates");

    builder.HasKey(x => x.Id);

     builder.Property(x => x.Title)
 .IsRequired()
           .HasMaxLength(300);

       builder.Property(x => x.Issuer)
  .IsRequired()
   .HasMaxLength(200);

            builder.Property(x => x.IssueDate);

          builder.Property(x => x.Description)
 .HasMaxLength(1000);

       builder.Property(x => x.ImageUrl)
           .HasMaxLength(500);

     builder.Property(x => x.VerificationUrl)
  .HasMaxLength(500);

 builder.Property(x => x.DisplayOrder)
  .IsRequired();

            builder.Property(x => x.IsPublished)
     .IsRequired();

        // Audit fields
      builder.Property(x => x.CreatedAtUtc).IsRequired();
     builder.Property(x => x.UpdatedAtUtc);

  // Soft delete
   builder.Property(x => x.IsDeleted).IsRequired();
     builder.Property(x => x.DeletedAtUtc);

     // Index
        builder.HasIndex(x => new { x.IsDeleted, x.IsPublished, x.DisplayOrder });
        }
 }
}
