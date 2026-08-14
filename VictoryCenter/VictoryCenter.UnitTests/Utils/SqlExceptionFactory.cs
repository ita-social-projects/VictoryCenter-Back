using System.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace VictoryCenter.UnitTests.Utils;

internal static class SqlExceptionFactory
{
    public static DbUpdateException CreateDbUpdateException(int errorNumber, string message)
    {
        var errorCollection = (SqlErrorCollection)Activator.CreateInstance(
            typeof(SqlErrorCollection),
            nonPublic: true)!;
        var errorConstructor = typeof(SqlError)
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .OrderByDescending(constructor => constructor.GetParameters().Length)
            .First();
        var errorArguments = errorConstructor.GetParameters()
            .Select(parameter => CreateSqlErrorArgument(parameter, errorNumber, message))
            .ToArray();
        var error = (SqlError)errorConstructor.Invoke(errorArguments);

        typeof(SqlErrorCollection)
            .GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(errorCollection, [error]);

        var createExceptionMethod = typeof(SqlException)
            .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .First(method => method.Name == "CreateException"
                && method.GetParameters().Length == 2);
        var sqlException = (SqlException)createExceptionMethod.Invoke(
            null,
            [errorCollection, "15.0.0"])!;

        return new DbUpdateException(message, sqlException);
    }

    private static object? CreateSqlErrorArgument(ParameterInfo parameter, int errorNumber, string message)
    {
        if (parameter.Name is "infoNumber" or "number")
        {
            return errorNumber;
        }

        if (parameter.ParameterType == typeof(byte))
        {
            return (byte)0;
        }

        if (parameter.ParameterType == typeof(int))
        {
            return 0;
        }

        if (parameter.ParameterType == typeof(uint))
        {
            return 0u;
        }

        if (parameter.ParameterType == typeof(string))
        {
            return parameter.Name == "errorMessage" ? message : string.Empty;
        }

        return null;
    }
}
