using ExternalFormatProcessing.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExternalFormatProcessing.Data.EntityConfig
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FirstName)
                .HasMaxLength(25)
                .IsRequired(false);

            builder.Property(x => x.LastName)
                .HasMaxLength(25)
                .IsRequired(true);

            builder.Property(x => x.Age)
                .IsRequired(false);

            builder.HasMany(x => x.ProductsBought)
                .WithOne(x => x.Buyer)
                .HasForeignKey(x => x.BuyerId);

            builder.HasMany(x => x.ProductsSold)
                .WithOne(x => x.Seller)
                .HasForeignKey(x => x.SellerId);
        }
    }
}
