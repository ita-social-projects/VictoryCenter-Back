using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Errors;

/// <summary>
/// Builds typed <see cref="NotFoundError"/> instances from the existing
/// <see cref="ErrorMessagesConstants"/> not-found message text, for use in
/// <c>Result.Fail(...)</c> call sites. Kept separate from <see cref="ErrorMessagesConstants"/>
/// because that class's methods also feed thrown exceptions and FluentValidation messages,
/// which need a plain string rather than an <see cref="FluentResults.IError"/>.
/// </summary>
public static class NotFoundErrors
{
    public static NotFoundError NotFound() => new(ErrorMessagesConstants.NotFound());

    public static NotFoundError NotFound(IEnumerable<long> ids, Type entityType) =>
        new(ErrorMessagesConstants.NotFound(ids, entityType));

    public static NotFoundError NotFound(object? id, Type entityType) =>
        new(ErrorMessagesConstants.NotFound(id, entityType));

    public static NotFoundError NotFoundByIdentifier(object? identifier, Type entityType) =>
        new(ErrorMessagesConstants.NotFoundByIdentifier(identifier, entityType));
}
