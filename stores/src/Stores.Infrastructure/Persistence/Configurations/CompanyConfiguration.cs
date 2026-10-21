using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stores.Domain;

namespace Stores.Infrastructure.Persistence.Configurations;

internal class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public const string UniqueNameIndex = "UX_Companies_Name";

    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(c => c.Name)
            .HasMaxLength(Company.NameMaxLength)
            .IsRequired();

        AuditColumns.Map(builder);

        builder.HasIndex(c => c.Name)
            .IsUnique()
            .HasFilter("[IsActive] = 1")
            .HasDatabaseName(UniqueNameIndex);
    }
}
