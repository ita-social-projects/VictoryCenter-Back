using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class CompanyProfileRequisite : BaseEntity
{
    public long ProfileId { get; set; }
    public CompanyProfile Profile { get; set; } = null!;
    public string? Recipient { get; set; }
    public string? Edrpou { get; set; }
    public string? Address { get; set; }
}
