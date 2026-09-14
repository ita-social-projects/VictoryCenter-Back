namespace VictoryCenter.BLL.Constants;

public static class VideoReviewConstants
{
    public static readonly int TitleMinLength = 5;
    public static readonly int TitleMaxLength = 200;
    public static readonly int LinkMinLength = 10;
    public static readonly int LinkMaxLength = 10000;

    public static readonly string LocalizationAlreadyExists =
        "A translation for this video review and language already exists";
}
