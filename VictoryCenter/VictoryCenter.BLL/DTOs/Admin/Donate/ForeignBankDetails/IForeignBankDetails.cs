namespace VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;

public interface IForeignBankDetails
{
    string Name { get; }
    string Receiver { get; }
    string Iban { get; }
    string Swift { get; }
    string Address { get; }
}
