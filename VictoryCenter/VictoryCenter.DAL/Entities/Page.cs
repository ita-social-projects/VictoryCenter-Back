namespace VictoryCenter.DAL.Entities;

public class Page
{
    public long Id { get; set; }

    public string Slug { get; set; }

    public string Title { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<FaqPageQuestion> Questions { get; set; } = [];
}
