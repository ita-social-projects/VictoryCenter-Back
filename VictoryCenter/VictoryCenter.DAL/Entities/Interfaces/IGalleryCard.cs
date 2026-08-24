namespace VictoryCenter.DAL.Entities.Interfaces;

public interface IGalleryCard
{
    string Description { get; set; }

    long? ImageId { get; set; }

    long Priority { get; set; }
}
