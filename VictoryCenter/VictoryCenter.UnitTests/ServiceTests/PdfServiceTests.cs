using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Services.PdfStorage;

namespace VictoryCenter.UnitTests.ServiceTests;

public class PdfServiceTests : IDisposable
{
    private readonly string _tempDir;
    private readonly string _subDir;
    private readonly PdfService _pdfService;
    private readonly Mock<IHttpContextAccessor> _mockHttpContext;
    private readonly PdfEnvironmentVariables _pdfEnv;
    private readonly string _fileName = "testfile";

    private static readonly byte[] ValidPdfBytes = { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x34 };

    public PdfServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _subDir = "PdfReports";

        _pdfEnv = new PdfEnvironmentVariables
        {
            RootPath = _tempDir,
            PdfSubPath = _subDir
        };

        _mockHttpContext = new Mock<IHttpContextAccessor>();
        _pdfService = new PdfService(Options.Create(_pdfEnv), _mockHttpContext.Object);
    }

    [Fact]
    public async Task UploadPdfAsync_ValidFile_ShouldCreateFileAndReturnBlobName()
    {
        // Arrange
        var file = CreateMockPdfFile();

        // Act
        var blobName = await _pdfService.UploadPdfAsync(file, _fileName);

        // Assert
        var filePath = Path.Combine(_pdfEnv.FullPath, $"{_fileName}.pdf");
        Assert.True(File.Exists(filePath));
        Assert.Equal($"{_fileName}.pdf", blobName);
    }

    [Fact]
    public async Task UploadPdfAsync_NullFile_ShouldThrowInvalidPdfFormatException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidPdfFormatException>(
            () => _pdfService.UploadPdfAsync(null!));
    }

    [Fact]
    public async Task UploadPdfAsync_EmptyFile_ShouldThrowInvalidPdfFormatException()
    {
        // Arrange
        var file = CreateMockPdfFile(content: []);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidPdfFormatException>(
            () => _pdfService.UploadPdfAsync(file));
    }

    [Theory]
    [InlineData("image/jpeg")]
    [InlineData("image/png")]
    [InlineData("text/plain")]
    [InlineData("application/msword")]
    public async Task UploadPdfAsync_WrongMimeType_ShouldThrowInvalidPdfFormatException(string contentType)
    {
        // Arrange
        var file = CreateMockPdfFile(contentType: contentType);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidPdfFormatException>(
            () => _pdfService.UploadPdfAsync(file));
    }

    [Fact]
    public async Task UploadPdfAsync_InvalidPdfSignature_ShouldThrowInvalidPdfFormatException()
    {
        var file = CreateMockPdfFile(content: [0x00, 0x01, 0x02, 0x03]);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidPdfFormatException>(
            () => _pdfService.UploadPdfAsync(file));
    }

    [Theory]
    [InlineData("file...pdf")]
    [InlineData("file/name")]
    [InlineData("file:name")]
    public async Task UploadPdfAsync_InvalidFileName_ShouldThrowBlobFileNameException(string invalidFileName)
    {
        // Arrange
        var file = CreateMockPdfFile();

        // Act & Assert
        await Assert.ThrowsAsync<BlobFileNameException>(
            () => _pdfService.UploadPdfAsync(file, invalidFileName));
    }

    [Fact]
    public async Task UploadPdfAsync_NoFileNameProvided_ShouldUseOriginalFileName()
    {
        // Arrange
        var file = CreateMockPdfFile(fileName: "original-report.pdf");

        // Act
        var blobName = await _pdfService.UploadPdfAsync(file); // fileName = null

        // Assert
        var filePath = Path.Combine(_pdfEnv.FullPath, "original-report.pdf");
        Assert.True(File.Exists(filePath));
        Assert.Equal("original-report.pdf", blobName);
    }

    [Fact]
    public async Task GetPdfAsync_ExistingFile_ShouldReturnCorrectContent()
    {
        // Arrange
        var file = CreateMockPdfFile();
        await _pdfService.UploadPdfAsync(file, _fileName);

        // Act
        using var stream = await _pdfService.GetPdfAsync($"{_fileName}.pdf");

        // Assert
        Assert.NotNull(stream);
        Assert.True(stream.Length > 0);
    }

    [Fact]
    public async Task GetPdfAsync_NonExistentFile_ShouldThrowBlobNotFoundException()
    {
        // Act & Assert
        var ex = await Assert.ThrowsAsync<BlobNotFoundException>(
            () => _pdfService.GetPdfAsync("nonexistent.pdf"));

        Assert.Contains("nonexistent", ex.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetPdfAsync_EmptyFileName_ShouldThrowBlobFileNameException(string fileName)
    {
        // Act & Assert
        await Assert.ThrowsAsync<BlobFileNameException>(
            () => _pdfService.GetPdfAsync(fileName));
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

    private static IFormFile CreateMockPdfFile(
        string contentType = "application/pdf",
        byte[] content = null!,
        string fileName = "test.pdf")
    {
        var fileContent = content ?? ValidPdfBytes;
        var mockFile = new Mock<IFormFile>();
        var stream = new MemoryStream(fileContent);

        mockFile.Setup(f => f.ContentType).Returns(contentType);
        mockFile.Setup(f => f.Length).Returns(fileContent.Length);
        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.OpenReadStream()).Returns(() => new MemoryStream(fileContent));
        mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns<Stream, CancellationToken>(async (s, _) =>
            {
                await new MemoryStream(fileContent).CopyToAsync(s);
            });

        return mockFile.Object;
    }
}
