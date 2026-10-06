using VictoryCenter.DbUpdate;

namespace VictoryCenter.UnitTests.DbUpdateTests;

public class WebApiProjectPathResolverTests
{
    [Fact]
    public void Resolve_FindsWebApiProjectFromSolutionDirectory()
    {
        var root = CreateTemporaryDirectory();

        try
        {
            var projectPath = Path.Combine(root, "VictoryCenter", "VictoryCenter.WebAPI");
            Directory.CreateDirectory(projectPath);
            File.WriteAllText(Path.Combine(projectPath, "appsettings.json"), "{}");

            var result = WebApiProjectPathResolver.Resolve(null, root, Path.Combine(root, "unused"));

            Assert.Equal(projectPath, result);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Resolve_AcceptsExplicitAppsettingsPath()
    {
        var root = CreateTemporaryDirectory();

        try
        {
            var configurationPath = Path.Combine(root, "appsettings.json");
            File.WriteAllText(configurationPath, "{}");

            var result = WebApiProjectPathResolver.Resolve(configurationPath, Path.GetTempPath(), Path.GetTempPath());

            Assert.Equal(root, result);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Resolve_FindsWebApiProjectFromExecutableDirectory()
    {
        var root = CreateTemporaryDirectory();

        try
        {
            var projectPath = Path.Combine(root, "VictoryCenter.WebAPI");
            Directory.CreateDirectory(projectPath);
            File.WriteAllText(Path.Combine(projectPath, "appsettings.json"), "{}");

            var result = WebApiProjectPathResolver.Resolve(
                null,
                Path.Combine(root, "unrelated"),
                Path.Combine(root, "VictoryCenter.DbUpdate", "bin"));

            Assert.Equal(projectPath, result);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Resolve_RejectsExplicitDirectoryWithoutAppsettings()
    {
        var root = CreateTemporaryDirectory();

        try
        {
            var exception = Assert.Throws<DirectoryNotFoundException>(() =>
                WebApiProjectPathResolver.Resolve(root, root, root));

            Assert.Contains("VICTORYCENTER_WEBAPI_PATH", exception.Message);
            Assert.Contains("appsettings.json", exception.Message);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Resolve_ReportsHowToConfigureMissingProject()
    {
        var root = CreateTemporaryDirectory();

        try
        {
            var exception = Assert.Throws<DirectoryNotFoundException>(() =>
                WebApiProjectPathResolver.Resolve(null, root, root));

            Assert.Contains("VictoryCenter.WebAPI/appsettings.json", exception.Message);
            Assert.Contains("VICTORYCENTER_WEBAPI_PATH", exception.Message);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static string CreateTemporaryDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), "VictoryCenter.DbUpdate.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
