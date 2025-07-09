using System.Text;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Services.BlobStorage;

namespace VictoryCenter.UnitTests.ServiceTests;

public class BlobServiceTests : IDisposable
{
    private readonly string _tempDir;
    private readonly BlobService _blobService;
    private readonly string _key = "testkey123";
    private readonly string _base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("test image content"));
    private readonly string _mimeType = "image/png";
    private readonly string _fileName = "testfile";

    public BlobServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var env = new BlobEnvironmentVariables
        {
            BlobStoreKey = _key,
            BlobStorePath = _tempDir
        };
        _blobService = new BlobService(Options.Create(env));
    }

    [Fact]
    public void SaveFileInStorage_ShouldCreateFile()
    {
        var blobName = _blobService.SaveFileInStorage(_base64, _fileName, _mimeType);
        var filePath = Path.Combine(_tempDir, $"{_fileName}.png");
        Assert.True(File.Exists(filePath));
        Assert.Equal($"{_fileName}.png", blobName);
    }

    [Fact]
    public void FindFileInStorageAsMemoryStream_ShouldReturnOriginalContent()
    {
        _blobService.SaveFileInStorage(_base64, _fileName, _mimeType);
        using var stream = _blobService.FindFileInStorageAsMemoryStream(_fileName, _mimeType);
        var content = Encoding.UTF8.GetString(stream.ToArray());
        Assert.Equal("test image content", content);
    }

    [Fact]
    public void FindFileInStorageAsBase64_ShouldReturnOriginalBase64()
    {
        // Arrange
        var blobName = _blobService.SaveFileInStorage(_base64, _fileName, _mimeType);
        Assert.NotNull(blobName); // Ensure blobName is not null

        // Construct the expected file path using _blobPath
        var filePath = Path.Combine(_blobService.BlobPath, blobName);

        // Ensure the directory exists
        Assert.True(Directory.Exists(_blobService.BlobPath), $"The directory '{_blobService.BlobPath}' does not exist.");

        // Ensure the file is saved correctly
        Assert.True(File.Exists(filePath), $"The file '{filePath}' was not created as expected.");

        // Act
        var base64 = _blobService.FindFileInStorageAsBase64(_fileName, _mimeType);
        var content = Encoding.UTF8.GetString(Convert.FromBase64String(base64));

        // Assert
        Assert.Equal("test image content", content);
    }

    [Fact]
    public void UpdateFileInStorage_ShouldReplaceFile()
    {
        _blobService.SaveFileInStorage(_base64, _fileName, _mimeType);
        var newContent = Convert.ToBase64String(Encoding.UTF8.GetBytes("new content"));
        _blobService.UpdateFileInStorage(_fileName, _mimeType, newContent, _fileName, _mimeType);

        using var stream = _blobService.FindFileInStorageAsMemoryStream(_fileName, _mimeType);
        var content = Encoding.UTF8.GetString(stream.ToArray());
        Assert.Equal("new content", content);
    }

    [Fact]
    public void DeleteFileInStorage_ShouldRemoveFile()
    {
        _blobService.SaveFileInStorage(_base64, _fileName, _mimeType);
        _blobService.DeleteFileInStorage(_fileName, _mimeType);
        var filePath = Path.Combine(_tempDir, $"{_fileName}.png");
        Assert.False(File.Exists(filePath));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, true);
        }
    }
}
