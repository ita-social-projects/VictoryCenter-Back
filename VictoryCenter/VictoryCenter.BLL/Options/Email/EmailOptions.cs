using System.Text.Json.Serialization;

namespace VictoryCenter.BLL.Options.Email;

public class EmailOptions
{
    public static readonly string Position = "EmailOptions";

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EmailProvider EmailProvider { get; init; }

    public string? ResendApiToken { get; init; }
}

public enum EmailProvider
{
    Resend,
    Dummy
}
