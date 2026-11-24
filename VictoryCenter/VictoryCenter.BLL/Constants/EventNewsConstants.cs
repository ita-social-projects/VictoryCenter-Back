namespace VictoryCenter.BLL.Constants;

public static class EventNewsConstants
{
    #pragma warning disable S2339
    public const int PublishedTakeMinValue = 1;
    public const int PublishedTakeMaxValue = 100;
    public const int TitleMinLength = 5;
    public const int TitleMaxLength = 60;
    public const int DescriptionMinLength = 5;
    public const int DescriptionMaxLength = 140;
    public const int AdditionalDescriptionMinLength = 2;
    public const int AdditionalDescriptionMaxLength = 20;
    public const int ResourceMaxLength = 150;
    #pragma warning restore S2339
    public static readonly int DefaultLimit = 20;
    public static readonly int PublishedOffsetMinValue = 0;
}
