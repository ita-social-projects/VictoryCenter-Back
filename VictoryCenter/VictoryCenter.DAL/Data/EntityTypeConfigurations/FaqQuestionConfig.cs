using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class FaqQuestionConfig : IEntityTypeConfiguration<FaqQuestion>
{
    public void Configure(EntityTypeBuilder<FaqQuestion> entity)
    {
        entity
            .HasKey(e => e.Id);

        entity
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity
            .Property(e => e.QuestionText)
            .IsRequired();

        entity
            .Property(e => e.AnswerText)
            .IsRequired();

        entity
            .Property(e => e.Status)
            .IsRequired();

        entity
            .Property(e => e.CreatedAt)
            .IsRequired();
    }
}
