using Dyt.Data.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dyt.Data.Configurations.Content
{
    /// <summary>
    /// ContactMessage entity yapýlandýrmasý
    /// </summary>
    public class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
    {
        public void Configure(EntityTypeBuilder<ContactMessage> builder)
        {
       builder.ToTable("ContactMessages");

       builder.HasKey(c => c.Id);

         builder.Property(c => c.Name)
 .IsRequired()
                .HasMaxLength(80);

            builder.Property(c => c.Email)
     .IsRequired()
           .HasMaxLength(160);

        builder.Property(c => c.Phone)
       .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.Subject)
    .IsRequired()
.HasMaxLength(120);

            builder.Property(c => c.Message)
   .IsRequired()
     .HasMaxLength(2000);

         builder.Property(c => c.KvkkConsent)
                .IsRequired();

            builder.Property(c => c.IsRead)
    .IsRequired()
     .HasDefaultValue(false);

   builder.Property(c => c.IsReplied)
       .IsRequired()
     .HasDefaultValue(false);

   builder.Property(c => c.AdminNote)
       .HasMaxLength(1000);

       // Ýndeksler
  builder.HasIndex(c => c.Email);
            builder.HasIndex(c => c.IsRead);
        builder.HasIndex(c => c.CreatedAtUtc);
        }
    }
}
