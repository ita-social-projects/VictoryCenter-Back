using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class EventNewsCategory : BaseEntity
{
    public string Name { get; set; } = null!;
    public ICollection<EventNews> EventsNews { get; set; } = [];
}
