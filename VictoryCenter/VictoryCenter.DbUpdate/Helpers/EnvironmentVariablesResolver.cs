using System.Text.RegularExpressions;

namespace VictoryCenter.DbUpdate.Helpers;

public static class EnvironmentVariablesResolver
{
    public static string GetEnvironmentVariable(string input)
    {
        return Regex.Replace(
            input,
            @"\$\{(.*?)\}",
            match =>
            {
                var envVar = match.Groups[1].Value;
                var envValue = Environment.GetEnvironmentVariable(envVar);
                if (string.IsNullOrEmpty(envValue))
                {
                    throw new InvalidOperationException($"Environment variable '{envVar}' is not set.");
                }

                return envValue;
            },
            RegexOptions.None,
            TimeSpan.FromSeconds(2));
    }
}
