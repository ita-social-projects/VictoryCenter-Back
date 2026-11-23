namespace VictoryCenter.BLL.Constants.Localization;

public static class HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants
{
    public static readonly int TitleMinLength = HippotherapyLandingPageConstants.TitleMinLength;
    public static readonly int TitleMaxLength = HippotherapyLandingPageConstants.HippoventionCenterTitleMaxLength;

    // "Додатковий опис" in US #2249 and #2688: optional, so length rules apply only when a value is entered.
    public static readonly int DescriptionMinLength = HippotherapyLandingPageConstants.TextMinLength;
    public static readonly int DescriptionMaxLength = HippotherapyLandingPageConstants.HippoventionCenterDescriptionMaxLength;

    public static readonly int ProsMinLength = HippotherapyLandingPageConstants.TextMinLength;
    public static readonly int ProsMaxLength = HippotherapyLandingPageConstants.HippoventionProsMaxLength;
}
