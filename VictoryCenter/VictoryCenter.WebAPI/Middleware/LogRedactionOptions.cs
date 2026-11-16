namespace VictoryCenter.WebAPI.Middleware;

public sealed class LogRedactionOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "LogRedaction";

    /// <summary>
    /// Gets or sets the extra query parameter names to redact, in addition to the built-in defaults.
    /// </summary>
    /// <value>
    /// Parameter names, matched case-insensitively against the URL-decoded name.
    /// </value>
    public string[] AdditionalSensitiveQueryParameters { get; set; } = [];
}
