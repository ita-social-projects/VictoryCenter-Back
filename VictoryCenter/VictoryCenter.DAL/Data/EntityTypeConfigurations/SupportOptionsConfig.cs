using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
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
            .IsRequired();

        builder.Property(e => e.Value)
            .IsRequired();

        builder.ToTable("SupportOptions");
    }
}
