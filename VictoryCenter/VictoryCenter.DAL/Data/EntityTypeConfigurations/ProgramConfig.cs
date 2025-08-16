using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class ProgramConfig : IEntityTypeConfiguration<Program>
{
    public void Configure(EntityTypeBuilder<Program> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .IsRequired();

        builder.Property(e => e.Description);

        builder.Property(e => e.ImageId);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired();

        builder.HasOne(e => e.Image)
            .WithOne()
            .HasForeignKey<Program>(e => e.ImageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(e => e.Categories)
            .WithMany(e => e.Programs)
            .UsingEntity(j =>
            {
                j.ToTable("ProgramProgramCategories");
            });
    }
}
