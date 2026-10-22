using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Products.Functions.Persistence.Configurations;

internal class StoreRowConfiguration : IEntityTypeConfiguration<StoreRow>
{
    public void Configure(EntityTypeBuilder<StoreRow> builder)
    {
        builder.ToTable("Stores");
        builder.HasNoKey();
    }
}

internal class CompanyRowConfiguration : IEntityTypeConfiguration<CompanyRow>
{
    public void Configure(EntityTypeBuilder<CompanyRow> builder)
    {
        builder.ToTable("Companies");
        builder.HasNoKey();
    }
}
