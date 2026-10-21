using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stores.Domain;

namespace Stores.Infrastructure.Persistence.Configurations;

internal class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public const string UniqueNameIndex = "UX_Stores_CompanyId_Name";

    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("Stores", t =>
        {
            t.HasCheckConstraint("CK_Stores_CountryCode", "[CountryCode] COLLATE Latin1_General_BIN LIKE '[A-Z][A-Z]'");
            t.HasCheckConstraint("CK_Stores_CurrencyCode", "[CurrencyCode] COLLATE Latin1_General_BIN LIKE '[A-Z][A-Z][A-Z]'");
        });

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(s => s.Name)
            .HasMaxLength(Store.NameMaxLength)
            .IsRequired();

        builder.Property(s => s.CountryCode)
            .HasColumnType("char(2)")
            .IsRequired();

        builder.Property(s => s.CurrencyCode)
            .HasColumnType("char(3)")
            .IsRequired();

        AuditColumns.Map(builder);

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(s => s.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => new { s.CompanyId, s.Name })
            .IsUnique()
            .HasFilter("[IsActive] = 1")
            .HasDatabaseName(UniqueNameIndex);

        builder.HasIndex(s => new { s.Id, s.CurrencyCode })
            .IsUnique()
            .HasDatabaseName("UX_Stores_Id_CurrencyCode");
    }
}
