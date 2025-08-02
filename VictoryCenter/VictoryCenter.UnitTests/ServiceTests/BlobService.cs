using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
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
    private readonly Mock<IHttpContextAccessor> _mockHttpContext;

    public BlobServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var env = new BlobEnvironmentVariables
        {
            BlobStoreKey = _key,
            BlobStorePath = _tempDir
        };
        _mockHttpContext = new Mock<IHttpContextAccessor>();

        _blobService = new BlobService(Options.Create(env), _mockHttpContext.Object);
    }

    [Fact]
    public async Task SaveFileInStorage_ShouldCreateFile()
    {
        var blobName = await _blobService.SaveFileInStorageAsync(_base64, _fileName, _mimeType);
        var filePath = Path.Combine(_tempDir, $"{_fileName}.png");
        Assert.True(File.Exists(filePath));
        Assert.Equal($"{_fileName}.png", blobName);
        var encryptedContent = File.ReadAllBytes(filePath);
        var originalContent = Convert.FromBase64String(_base64);
        Assert.Equal(originalContent, encryptedContent);
    }

    [Fact]
    public async Task FindFileInStorageAsMemoryStream_ShouldReturnOriginalContent()
    {
        await _blobService.SaveFileInStorageAsync(_base64, _fileName, _mimeType);
        using var stream = await _blobService.FindFileInStorageAsMemoryStreamAsync(_fileName, _mimeType);
        var content = Encoding.UTF8.GetString(stream.ToArray());
        Assert.Equal("test image content", content);
    }

    [Fact]
    public async Task GetFileUrl_ShouldReturnOriginalBase64()
    {
        var httpRequestMock = new Mock<HttpRequest>();
        var httpContextMock = new Mock<HttpContext>();
        httpRequestMock.Setup(r => r.Scheme).Returns("https");
        httpRequestMock.Setup(r => r.Host).Returns(new HostString("example.com"));
        httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);
        _mockHttpContext.Setup(h => h.HttpContext).Returns(httpContextMock.Object);

        var blobName = "image123";
        var mimeType = "image/png";

        var result = _blobService.GetFileUrl(blobName, mimeType);

        Assert.Equal("https://example.com/image123.png", result);
    }

    [Fact]
    public async Task UpdateFileInStorage_ShouldReplaceFile()
    {
        await _blobService.SaveFileInStorageAsync(_base64, _fileName, _mimeType);
        var newContent = Convert.ToBase64String(Encoding.UTF8.GetBytes("new content"));
        await _blobService.UpdateFileInStorageAsync(_fileName, _mimeType, newContent, _fileName, _mimeType);

        using var stream = await _blobService.FindFileInStorageAsMemoryStreamAsync(_fileName, _mimeType);
        var content = Encoding.UTF8.GetString(stream.ToArray());
        Assert.Equal("new content", content);
    }

    [Fact]
    public async Task DeleteFileInStorage_ShouldRemoveFile()
    {
        await _blobService.SaveFileInStorageAsync(_base64, _fileName, _mimeType);
        _blobService.DeleteFileInStorage(_fileName, _mimeType);
        var filePath = Path.Combine(_tempDir, $"{_fileName}.png");
        Assert.False(File.Exists(filePath));
    }

    [Fact]
    public void DeleteFileInStorage_NonExistentFile_ShouldNotThrow()
        {
            var exception = Record.Exception(() => _blobService.DeleteFileInStorage("nonexistent", _mimeType));
            Assert.Null(exception);
        }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, true);
        }
    }
}
