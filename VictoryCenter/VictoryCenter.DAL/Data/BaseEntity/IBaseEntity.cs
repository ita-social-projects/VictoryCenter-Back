namespace VictoryCenter.DAL.Data.BaseEntity;
public interface IBaseEntity
{
    long Id { get; set; }
    DateTime CreatedAt { get; set; }
}
