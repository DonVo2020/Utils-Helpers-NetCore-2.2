using AdvancedRelations.Models;
using Microsoft.EntityFrameworkCore;


namespace AdvancedRelations.EntityConfig
{
    public class BankAccountConfig : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<BankAccount> builder)
        {
            builder.HasKey(x => x.BankAccountId);

            builder.Property(x => x.BankName)
                .IsUnicode()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.SWIFTCode)
                .IsUnicode(false)
                .HasMaxLength(25)
                .IsRequired();
        }
    }
}
