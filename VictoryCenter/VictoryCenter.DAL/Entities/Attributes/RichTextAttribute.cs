namespace VictoryCenter.DAL.Entities.Attributes;

/// <summary>
/// Marks a property as rich-text HTML, sanitized against the allowlist before saving.
/// Mark the matching localization entity property too.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RichTextAttribute : Attribute
{
}
