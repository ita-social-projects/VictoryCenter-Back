using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class SupportOptionsConfig : IEntityTypeConfiguration<SupportOptions>
{
    public void Configure(EntityTypeBuilder<SupportOptions> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Value)
            .IsRequired()
            .HasMaxLength(100);

        builder.ToTable("SupportOptions");
    }
}
