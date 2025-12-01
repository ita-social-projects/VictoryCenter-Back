using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class HippotherapyProgramConfig : IEntityTypeConfiguration<HippotherapyProgram>
{
    public void Configure(EntityTypeBuilder<HippotherapyProgram> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .IsRequired();

        builder.Property(e => e.Description);

        builder.Property(e => e.Status)
            .IsRequired();

        builder.Property(e => e.Location);

        builder.Property(e => e.ParticipantsCount);

        builder.Property(e => e.MeetingsCount);

        builder.Property(e => e.BackgroundImageId);

        builder.Property(e => e.PreviewImageId);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder
            .HasOne(e => e.BackgroundImage)
            .WithOne()
            .HasForeignKey<HippotherapyProgram>(e => e.BackgroundImageId);

        builder
            .HasOne(e => e.PreviewImage)
            .WithOne()
            .HasForeignKey<HippotherapyProgram>(e => e.PreviewImageId);

        builder.HasMany(e => e.Categories)
            .WithMany(e => e.Programs)
            .UsingEntity(j =>
            {
                j.ToTable("ProgramsProgramCategories");
            });
    }
}
