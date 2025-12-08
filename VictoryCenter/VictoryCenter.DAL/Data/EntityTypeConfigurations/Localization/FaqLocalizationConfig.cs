using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class FaqLocalizationConfig : EntityLocalizationConfig<FaqQuestionLocalization, FaqQuestion>
{
    public override void Configure(EntityTypeBuilder<FaqQuestionLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.QuestionText)
            .IsRequired();

        entity.Property(e => e.AnswerText)
            .IsRequired();
    }
}
