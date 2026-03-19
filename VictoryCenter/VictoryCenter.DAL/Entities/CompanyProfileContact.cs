using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class CompanyProfileContact : BaseEntity
{
    public long ProfileId { get; set; }
    public CompanyProfile Profile { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }
    public string? CorrespondenceEmail { get; set; }
    public string? Motto { get; set; }
}
