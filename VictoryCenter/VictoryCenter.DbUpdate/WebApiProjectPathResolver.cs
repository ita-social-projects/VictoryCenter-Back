namespace VictoryCenter.DbUpdate;

internal static class WebApiProjectPathResolver
{
    private const string ProjectDirectoryName = "VictoryCenter.WebAPI";

    public static string Resolve(string? configuredPath, string workingDirectory, string executableDirectory)
    {
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            var fullPath = Path.GetFullPath(configuredPath);
            var projectPath = Path.GetFileName(fullPath) == "appsettings.json"
                ? Path.GetDirectoryName(fullPath)!
                : fullPath;

            if (File.Exists(Path.Combine(projectPath, "appsettings.json")))
            {
                return projectPath;
            }

            throw new DirectoryNotFoundException(
                $"VICTORYCENTER_WEBAPI_PATH '{configuredPath}' does not contain appsettings.json.");
        }

        foreach (var startDirectory in new[] { workingDirectory, executableDirectory })
        {
            for (var directory = new DirectoryInfo(startDirectory); directory != null; directory = directory.Parent)
            {
                foreach (var candidate in new[]
                {
                    Path.Combine(directory.FullName, ProjectDirectoryName),
                    Path.Combine(directory.FullName, "VictoryCenter", ProjectDirectoryName),
                })
                {
                    if (File.Exists(Path.Combine(candidate, "appsettings.json")))
                    {
                        return candidate;
                    }
                }
            }
        }

        throw new DirectoryNotFoundException(
            $"Could not find {ProjectDirectoryName}/appsettings.json from '{workingDirectory}' or '{executableDirectory}'. " +
            "Set VICTORYCENTER_WEBAPI_PATH to the WebAPI project directory or its appsettings.json file.");
    }
}
