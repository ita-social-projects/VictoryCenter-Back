namespace VictoryCenter.BLL.Constants;

public static class MainPageConstants
{
    public static class Title
    {
        public static int MinLength => 5;
        public static int MaxLength => 150;
    }

    public static class Description
    {
        public static int MinLength => 10;
        public static int MaxLength => 1000;
    }

    public static class Localization
    {
        public static class Title
        {
            public static int MinLength => 10;
            public static int MaxLength => 50;
        }

        public static class TitleBlockDescription
        {
            public static int MinLength => 10;
            public static int MaxLength => 300;
        }

        public static class SectionDescription
        {
            public static int MinLength => 10;
            public static int MaxLength => 1000;
        }
    }

    public static class ImpactStatistic
    {
        public static int ExactMetricCount => 4;

        public static class TitleConstraints
        {
            public static int MinLength => 5;
            public static int MaxLength => 500;
        }
    }

    public static class Metric
    {
        public static class Name
        {
            public static int MinLength => 1;
            public static int MaxLength => 100;
        }
    }
}
