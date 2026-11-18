namespace VictoryCenter.DAL.Entities.Attributes;

/// <summary>
/// Marks an entity whose changes are written to the audit log.
/// Only properties marked with <see cref="AuditedAttribute"/> are recorded.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class AuditableAttribute : Attribute
{
}
