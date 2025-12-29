using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class HippotherapyProgramSectionConfig : IEntityTypeConfiguration<HippotherapyProgramSection>
{
    public void Configure(EntityTypeBuilder<HippotherapyProgramSection> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.ProgramId)
            .IsRequired();

        entity.Property(e => e.Template)
            .IsRequired();

        entity.Property(e => e.Order)
            .IsRequired();

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.HasOne(e => e.Program)
            .WithMany(p => p.Sections)
            .HasForeignKey(e => e.ProgramId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(e => e.Contents)
            .WithOne(c => c.Section)
            .HasForeignKey(c => c.SectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
