using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Services.BlobStorage;

namespace VictoryCenter.UnitTests.ServiceTests;

public class BlobServiceTests : IDisposable
{
    private readonly string _tempDir;
    private readonly string _subDir;
    private readonly BlobService _blobService;
    private readonly string _key = "testkey123";
    private readonly string _base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("test image content"));
    private readonly string _mimeType = "image/png";
    private readonly string _fileName = "testfile";
    private readonly Mock<IHttpContextAccessor> _mockHttpContext;
    private readonly BlobEnvironmentVariables _blobEnv;

    public BlobServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _subDir = "Images";
        var env = new BlobEnvironmentVariables
        {
            RootPath = _tempDir,
            ImagesSubPath = _subDir
        };
        _blobEnv = env;
        _mockHttpContext = new Mock<IHttpContextAccessor>();

        _blobService = new BlobService(Options.Create(env), _mockHttpContext.Object);
    }

    [Fact]
    public async Task SaveFileInStorage_ShouldCreateFile()
    {
        var blobName = await _blobService.SaveFileInStorageAsync(_base64, _fileName, _mimeType);
        var filePath = Path.Combine(Path.Combine(_tempDir, _subDir), $"{_fileName}.png");
        Assert.True(File.Exists(filePath));
        Assert.Equal($"{_fileName}.png", blobName);
        var encryptedContent = File.ReadAllBytes(filePath);
        var originalContent = Convert.FromBase64String(_base64);
        Assert.Equal(originalContent, encryptedContent);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("image...png")]
    public async Task SaveFileInStorage_WrongFileName_ShouldThrowException(string fileName)
    {
        var ex = await Assert.ThrowsAsync<BlobFileNameException>(
            () => _blobService.SaveFileInStorageAsync(_base64, fileName, _mimeType));

        Assert.Contains(fileName, ex.Message);
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

        Assert.Equal("https://example.com/Images/image123.png", result);
    }

    [Fact]
    public void GetFileUrl_InvalidHttpPath_ShouldThrowException()
    {
        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(c => c.Request).Returns((HttpRequest)null);
        _mockHttpContext.Setup(h => h.HttpContext).Returns(httpContextMock.Object);

        var blobName = "image123";
        var mimeType = "image/png";

        Assert.Throws<BlobFileSystemException>(() =>
            _blobService.GetFileUrl(blobName, mimeType));
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

    [Fact]
    public async Task SaveFileInStorage_InvalidBase64_ShouldThrowInvalidBase64FormatException()
    {
        var invalidBase64 = "ab#";

        var ex = await Assert.ThrowsAsync<InvalidBase64FormatException>(
            () => _blobService.SaveFileInStorageAsync(invalidBase64, _fileName, _mimeType));

        Assert.Equal(ImageConstants.InvalidBase64String, ex.Message);
    }

    [Fact]
    public async Task FindFileInStorage_NonExistentFile_ShouldThrowBlobNotFoundException()
    {
        var ex = await Assert.ThrowsAsync<BlobNotFoundException>(
            () => _blobService.FindFileInStorageAsMemoryStreamAsync("nonexistent", _mimeType));

        Assert.Contains("nonexistent", ex.Message);
    }

    [Fact]
    public async Task FindFileInStorage_FileIsLocked_ShouldThrowImageProcessingException()
    {
        var filePath = Path.Combine(_blobEnv.FullPath, $"{_fileName}.png");
        await _blobService.SaveFileInStorageAsync(_base64, _fileName, _mimeType);

        using (File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            var ex = await Assert.ThrowsAsync<ImageProcessingException>(
                () => _blobService.FindFileInStorageAsMemoryStreamAsync(_fileName, _mimeType));

            Assert.Equal(ImageConstants.FailedToReadImage, ex.Message);
            Assert.IsType<IOException>(ex.InnerException);
        }
    }

    [Fact]
    public async Task DeleteFileInStorage_FileIsReadOnly_ShouldThrowBlobFileSystemException()
    {
        var filePath = Path.Combine(_blobEnv.FullPath, $"{_fileName}.png");
        await _blobService.SaveFileInStorageAsync(_base64, _fileName, _mimeType);

        File.SetAttributes(filePath, FileAttributes.ReadOnly);

        try
        {
            var ex = Assert.Throws<BlobFileSystemException>(
                () => _blobService.DeleteFileInStorage(_fileName, _mimeType));

            Assert.Equal(ImageConstants.FailToDeleteImage, ex.Message);
            Assert.IsType<UnauthorizedAccessException>(ex.InnerException);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
            }
        }
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            var directory = new DirectoryInfo(_tempDir) { Attributes = FileAttributes.Normal };
            foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                info.Attributes = FileAttributes.Normal;
            }

            directory.Delete(true);
        }
    }
}
