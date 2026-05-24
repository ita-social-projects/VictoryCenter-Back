using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace VictoryCenter.WebAPI.Extensions;

public static class ConfigurationExtensions
{
    public static T GetValidated<T>(this IConfiguration configuration, string sectionName)
        where T : class
    {
        var options = configuration.GetSection(sectionName).Get<T>();

        if (options == null)
        {
            throw new InvalidOperationException(
                $"Configuration section '{sectionName}' could not be found or is empty.");
        }

        var validationContext = new ValidationContext(options);
        var validationResults = new List<ValidationResult>();

        if (!Validator.TryValidateObject(options, validationContext, validationResults, true))
        {
            var errors = validationResults
                .Select(r => r.ErrorMessage ?? $"Validation failed for {typeof(T).Name}.")
                .ToList();

            throw new OptionsValidationException(sectionName, typeof(T), errors);
        }

        return options;
    }
}
