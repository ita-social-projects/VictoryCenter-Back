using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Constants;

public static class PageConstants
{
    public static readonly List<VisitorPage> VisitorPages =
        [
            new() { Title = "Програми", Slug = "program-page" },
            new() { Title = "Про іпотерапію", Slug = "about-hippotherapy" },
            new() { Title = "Донати", Slug = "donate-page" }
        ];
}
