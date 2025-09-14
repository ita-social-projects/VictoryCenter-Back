namespace VictoryCenter.DAL.Entities.Interfaces;

public interface IOrderableEntity<TKey>
    where TKey : struct
{
    TKey Id { get; set; }
    TKey? NextElementId { get; set; }
}
