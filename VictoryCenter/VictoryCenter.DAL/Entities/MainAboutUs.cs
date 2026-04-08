using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class MainAboutUs : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;

    public long MainPageId { get; set; }
    public MainPage MainPage { get; set; } = null!;
}
