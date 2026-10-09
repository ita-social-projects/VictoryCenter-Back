namespace VictoryCenter.BLL.Constants.Localization;

public static class HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants
{
    public static readonly int TitleMinLength = HippotherapyLandingPageConstants.TitleMinLength;
    public static readonly int TitleMaxLength = HippotherapyLandingPageConstants.HippoventionCenterTitleMaxLength;

    public static readonly int DescriptionMinLength = HippotherapyLandingPageConstants.TextMinLength;
    public static readonly int DescriptionMaxLength = HippotherapyLandingPageConstants.HippoventionCenterDescriptionMaxLength;

    // Mirrors Pros in UpdateHippoventionCenterSectionDtoValidator (required, max 300); US #2688 lists it as optional with max 50.
    // If that changes, update both validators so the original and the translation accept the same values.
    public static readonly int ProsMinLength = HippotherapyLandingPageConstants.TextMinLength;
    public static readonly int ProsMaxLength = HippotherapyLandingPageConstants.HippoventionProsMaxLength;
}
