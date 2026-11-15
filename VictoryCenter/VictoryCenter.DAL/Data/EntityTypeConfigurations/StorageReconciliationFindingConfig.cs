using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class StorageReconciliationFindingConfig : IEntityTypeConfiguration<StorageReconciliationFinding>
{
    public void Configure(EntityTypeBuilder<StorageReconciliationFinding> entity)
    {
        entity.ToTable("StorageReconciliationFindings", "media");
        entity.HasKey(finding => finding.Id);
        entity.Property(finding => finding.FileName).HasMaxLength(StorageFileNames.MaxLength).IsRequired();
        entity.Property(finding => finding.CleanupOperationId).IsConcurrencyToken();
        entity.HasIndex(finding => new { finding.FileKind, finding.FileName, finding.Kind }).IsUnique();
    }
}
