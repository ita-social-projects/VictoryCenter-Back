namespace VictoryCenter.BLL.DTOs.Donations.Common;

public class AdditionalFieldDto
{
    public long Id { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string FieldValue { get; set; } = string.Empty;
}
