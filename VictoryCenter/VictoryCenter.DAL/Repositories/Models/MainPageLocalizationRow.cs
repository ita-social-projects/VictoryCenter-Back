namespace VictoryCenter.DAL.Repositories.Models;

public sealed class MainPageLocalizationRow
{
    public MainPageTranslationRow? Translation { get; init; }
    public MainPageTranslationRow? AboutUs { get; init; }
    public MainPageTranslationRow? Partners { get; init; }
    public MainPageTranslationRow? Donations { get; init; }
}
