namespace VictoryCenter.DAL.Entities.Interfaces;

public interface IOrderableEntity
{
    long Priority { get; set; }
}

/*public interface ILinkOrderableEntity<TKey>
    where TKey : struct
{
    TKey Id { get; set; }
    TKey? NextElementId { get; set; }
}

public interface ILinkOrderableEntity
{
    long? NextElementId { get; set; }
}
*/
