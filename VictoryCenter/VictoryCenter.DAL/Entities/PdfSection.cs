using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class PdfSection : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
}
