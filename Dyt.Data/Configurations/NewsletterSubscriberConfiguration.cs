using Dyt.Data.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dyt.Data.Configurations
{
    /// <summary>
    /// NewsletterSubscriber entity konfigürasyonu
    /// </summary>
    public class NewsletterSubscriberConfiguration : IEntityTypeConfiguration<NewsletterSubscriber>
    {
        public void Configure(EntityTypeBuilder<NewsletterSubscriber> builder)
        {
   builder.ToTable("NewsletterSubscribers");

       builder.HasKey(n => n.Id);

    builder.Property(n => n.Email)
                .IsRequired()
      .HasMaxLength(256);

      builder.HasIndex(n => n.Email)
                .IsUnique();

        builder.Property(n => n.UnsubscribeToken)
   .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(n => n.UnsubscribeToken)
          .IsUnique();

            builder.Property(n => n.IsActive)
      .IsRequired()
   .HasDefaultValue(true);

     builder.Property(n => n.IsVerified)
      .IsRequired()
         .HasDefaultValue(true);
  }
    }
}
