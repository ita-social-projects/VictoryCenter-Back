using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public abstract class EntityLocalizationConfig<TEntityLocalization, TEntity>
    : IEntityTypeConfiguration<TEntityLocalization>
    where TEntityLocalization : LocalizationBase<TEntity>
    where TEntity : class, ITranslatedEntity<TEntityLocalization>
{
    public virtual void Configure(EntityTypeBuilder<TEntityLocalization> entity)
    {
        entity.HasKey(tl => new { tl.EntityId, tl.LanguageId });

        entity.HasOne(tl => tl.Entity)
            .WithMany(t => t.Localizations)
            .HasForeignKey(tl => tl.EntityId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(tl => tl.Language)
            .WithMany()
            .HasForeignKey(tl => tl.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.Property(e => e.TranslationStatus)
            .IsRequired()
            .HasDefaultValue(TranslationStatus.Relevant);

        entity
            .Property(e => e.CreatedAt)
            .IsRequired();
    }
}
