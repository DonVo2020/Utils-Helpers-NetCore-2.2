using ExternalFormatProcessing.Models;
using Microsoft.EntityFrameworkCore;

namespace ExternalFormatProcessing.Data.EntityConfig
{
    internal class CategoryProductConfiguration : IEntityTypeConfiguration<CategoryProduct>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CategoryProduct> builder)
        {
            builder.HasKey(x => new { x.CategoryId, x.ProductId });
        }
    }
}
