namespace VictoryCenter.DAL.Data.BaseEntity;
public abstract class BaseEntity
{
    public long Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
