namespace VictoryCenter.BLL.Constants.Localization;

public static class HippotherapyLandingPageAnotherQuoteSectionLocalizationConstants
{
    public static readonly int QuoteTextMinLength = HippotherapyLandingPageConstants.TextMinLength;
    public static readonly int QuoteTextMaxLength = HippotherapyLandingPageConstants.QuoteTextMaxLength;

    // Mirrors the AuthorName minimum in UpdateAnotherQuoteSectionDtoValidator; US #2687 defines only the max length.
    // If the minimum changes, update both validators so the original and the translation accept the same values.
    public static readonly int AuthorNameMinLength = HippotherapyLandingPageConstants.TextMinLength;
    public static readonly int AuthorNameMaxLength = HippotherapyLandingPageConstants.QuoteAuthorNameMaxLength;
}
