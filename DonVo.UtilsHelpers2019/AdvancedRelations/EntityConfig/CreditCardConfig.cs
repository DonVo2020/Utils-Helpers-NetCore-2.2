using AdvancedRelations.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvancedRelations.EntityConfig
{
    public class CreditCardConfig : IEntityTypeConfiguration<CreditCard>
    {
        public void Configure(EntityTypeBuilder<CreditCard> builder)
        {
            builder.HasKey(x => x.CreditCardId);

            builder.Property(x => x.Limit)
                .IsRequired();

            builder.Property(x => x.MoneyOwed)
                .IsRequired();

            builder.Ignore(x => x.LimitLeft);
        }
    }
}
