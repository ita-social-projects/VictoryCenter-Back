using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class SupportMethodConfig : IEntityTypeConfiguration<SupportMethod>
{
    public void Configure(EntityTypeBuilder<SupportMethod> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Currency)
            .IsRequired();

        entity.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
