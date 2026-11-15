using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class StorageOperationConfig : IEntityTypeConfiguration<StorageOperation>
{
    public void Configure(EntityTypeBuilder<StorageOperation> entity)
    {
        entity.ToTable("StorageOperations", "media");
        entity.HasKey(operation => operation.Id);
        entity.Property(operation => operation.FileName).HasMaxLength(StorageFileNames.MaxLength).IsRequired();
        entity.Property(operation => operation.LastErrorCode).HasMaxLength(100);
        entity.Property(operation => operation.RequestedBy).HasMaxLength(200);
        entity.Property(operation => operation.Version).IsConcurrencyToken();
        entity.HasIndex(operation => new { operation.Status, operation.NextAttemptAt });
        entity.HasIndex(operation => new { operation.FileKind, operation.FileName, operation.Kind });
        entity.HasIndex(operation => new { operation.FileKind, operation.FileName })
            .IsUnique().HasFilter($"[Kind] = {(int)StorageOperationKind.UploadCleanup}");
    }
}
