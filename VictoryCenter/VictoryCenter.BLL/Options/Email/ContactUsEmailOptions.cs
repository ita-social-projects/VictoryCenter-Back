using System.ComponentModel.DataAnnotations;

namespace VictoryCenter.BLL.Options.Email;

public class ContactUsEmailOptions
{
    public static readonly string Position = "ContactUsEmailOptions";

    [Required]
    [EmailAddress]
    public required string FromAddress { get; init; }
}
