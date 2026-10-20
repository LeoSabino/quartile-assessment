using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Stores.Infrastructure.Persistence.Configurations;

internal static class AuditColumns
{
    public static void Map<T>(EntityTypeBuilder<T> builder) where T : class
    {
        builder.Property<DateTime>("CreatedAt")
            .HasColumnType("datetime2(3)")
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property<DateTime?>("UpdatedAt")
            .HasColumnType("datetime2(3)");

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsRequired();
    }
}
