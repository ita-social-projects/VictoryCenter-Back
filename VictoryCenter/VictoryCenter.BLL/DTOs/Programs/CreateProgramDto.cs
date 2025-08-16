using VictoryCenter.DAL.Enums;
namespace VictoryCenter.BLL.DTOs.Programs;

public class CreateProgramDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public Status Status { get; set; }
    public long? ImageId { get; set; }
    public List<long> CategoriesId { get; set; }
}
