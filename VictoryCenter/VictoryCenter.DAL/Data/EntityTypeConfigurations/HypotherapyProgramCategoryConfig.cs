using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class HypotherapyProgramCategoryConfig : IEntityTypeConfiguration<HypotherapyProgramCategory>
{
    public void Configure(EntityTypeBuilder<HypotherapyProgramCategory> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
