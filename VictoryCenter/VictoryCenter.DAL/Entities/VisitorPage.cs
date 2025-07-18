namespace VictoryCenter.DAL.Entities;

public class VisitorPage
{
    public long Id { get; set; }

    public string Slug { get; set; } = default!;

    public string Title { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public ICollection<FaqPlacement> Questions { get; set; } = [];
}
