namespace VictoryCenter.BLL.Constants;

public static class PdfSectionConstants
{
    public static readonly string SectionNotFound = "PDF section not found";

    public static readonly int TitleMinLength = 2;
    public static readonly int TitleMaxLength = 30;
    public static readonly string TitleMinLengthErrorMessage = "Не менше 2 символів";
    public static readonly string TitleMaxLengthErrorMessage = "Не більше 30 символів";
    public static readonly string TitleRequiredErrorMessage = "Поле обов'язкове";

    public static readonly int DescriptionMinLength = 2;
    public static readonly int DescriptionMaxLength = 160;
    public static readonly string DescriptionMinLengthErrorMessage = "Не менше 2 символів";
    public static readonly string DescriptionMaxLengthErrorMessage = "Не більше 160 символів";
    public static readonly string DescriptionRequiredErrorMessage = "Поле обов'язкове";
}
