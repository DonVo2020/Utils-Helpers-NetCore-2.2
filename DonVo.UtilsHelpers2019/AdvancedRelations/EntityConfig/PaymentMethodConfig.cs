using AdvancedRelations.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvancedRelations.EntityConfig
{
    public class PaymentMethodConfig : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.HasKey(x => x.PaymentId);

            builder.HasOne(u => u.User).WithMany(p => p.PaymentMethods).HasForeignKey(u => u.UserId);

            builder.HasOne(b => b.BankAccount).WithOne(p => p.PaymentMethod).HasForeignKey<PaymentMethod>(b => b.BankAccountId);

            builder.HasOne(c => c.CreditCard).WithOne(p => p.PaymentMethod).HasForeignKey<PaymentMethod>(c => c.CreditCardId);
        }
    }
}
