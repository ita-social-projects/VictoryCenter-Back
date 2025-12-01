namespace VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;

public abstract record BaseForeignBankDetailsDto()
{
    public string Name { get; set; } = null!;
    public string Receiver { get; set; } = null!;
    public string UkrainianIban { get; set; } = null!;
    public string Swift { get; set; } = null!;
    public string Address { get; set; } = null!;
}
