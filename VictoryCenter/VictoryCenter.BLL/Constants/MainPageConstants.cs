namespace VictoryCenter.BLL.Constants;

public static class MainPageConstants
{
    public static class Title
    {
        public const int MinLength = 5;
        public const int MaxLength = 150;
    }

    public static class Description
    {
        public const int MinLength = 10;
        public const int MaxLength = 1000;
    }

    public static class ImpactStatistic
    {
        public const int ExactMetricCount = 4;

        public static class TitleConstraints
        {
            public const int MinLength = 5;
            public const int MaxLength = 500;
        }
    }

    public static class Metric
    {
        public static class Name
        {
            public const int MinLength = 1;
            public const int MaxLength = 100;
        }
    }
}
