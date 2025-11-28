namespace VictoryCenter.DAL.Entities.Interfaces;

public interface ITranslatedEntity<TLocalizationEntity>
{
    ICollection<TLocalizationEntity> Localizations { get; set; }
}
