using FluentResults;

namespace VictoryCenter.BLL.Errors;

/// <summary>
/// Validation failure that keeps the FluentValidation property name, so API responses
/// can group messages by field instead of flattening them into a single detail string.
/// </summary>
public class ValidationError : Error
{
    public ValidationError(string? propertyName, string message)
        : base(message)
    {
        PropertyName = propertyName ?? string.Empty;
    }

    public string PropertyName { get; }
}
