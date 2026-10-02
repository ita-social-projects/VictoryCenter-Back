using VictoryCenter.DAL.Entities.Interfaces;

namespace VictoryCenter.DAL.Entities;

public class EventNewsEventNewsCategories : IOrderableEntity
{
    public long CategoriesId { get; set; }
    public long EventsNewsId { get; set; }
    public long Priority { get; set; }
}
