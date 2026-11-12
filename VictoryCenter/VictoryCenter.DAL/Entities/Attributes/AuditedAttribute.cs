namespace VictoryCenter.DAL.Entities.Attributes;

/// <summary>
/// Allows the property's old and new values to be written to the audit log.
/// Never mark credentials, tokens, payment details or file contents.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class AuditedAttribute : Attribute
{
}
