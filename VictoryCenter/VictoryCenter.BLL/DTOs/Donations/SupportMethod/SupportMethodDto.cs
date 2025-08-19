using VictoryCenter.BLL.DTOs.Donations.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Donations.SupportMethod;

public class SupportMethodDto
{
    public long Id { get; set; }
    public Currency Currency { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<AdditionalFieldDto> AdditionalFields { get; set; } = new();
}
