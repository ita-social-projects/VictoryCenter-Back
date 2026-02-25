using System.Transactions;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Moq;
using VictoryCenter.BLL.Commands.Admin.PdfReports.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.PdfStorage;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.BLL.Validators.PdfReports;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.PdfReports;

public class CreatePdfReportHandlerTests
{
    private readonly Mock<IPdfService> _mockPdfService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IReorderService> _mockReorderService;
    private readonly IValidator<CreatePdfReportCommand> _validator;

    private readonly IFormFile _testFile;
    private readonly PdfReport _testPdfReport;
    private readonly PdfReportDto _testPdfReportDto;

    public CreatePdfReportHandlerTests()
    {
        _validator = new CreatePdfReportValidator();
        _mockPdfService = new Mock<IPdfService>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockReorderService = new Mock<IReorderService>();

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.ContentType).Returns("application/pdf");
        mockFile.Setup(f => f.Length).Returns(1024);
        mockFile.Setup(f => f.FileName).Returns("test-report.pdf");
        _testFile = mockFile.Object;

        _testPdfReport = new PdfReport
        {
            Id = 1,
            Name = "test-report",
            BlobName = "abc123.pdf",
            FileSizeBytes = 1024,
            Priority = 1,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _testPdfReportDto = new PdfReportDto
        {
            Id = 1,
            Name = "test-report",
            BlobName = "abc123.pdf",
            FileSizeBytes = 1024,
            Priority = 1,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCreatePdfReportAndReturnDto()
    {
        // Arrange
        _mockPdfService.Setup(x => x.UploadPdfAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
            .ReturnsAsync("abc123.pdf");

        _mockReorderService.Setup(x => x.GetNextDisplayOrderAsync<PdfReport>(It.IsAny<System.Linq.Expressions.Expression<System.Func<PdfReport, bool>>>()))
            .ReturnsAsync(1);

        _mockRepositoryWrapper.Setup(x => x.PdfReportRepository.CreateAsync(It.IsAny<PdfReport>()))
            .ReturnsAsync(_testPdfReport);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _mockRepositoryWrapper.Setup(x => x.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        _mockMapper.Setup(x => x.Map<PdfReportDto>(It.IsAny<PdfReport>()))
            .Returns(_testPdfReportDto);

        var command = new CreatePdfReportCommand(new CreatePdfReportDto { File = _testFile });

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_testPdfReportDto.Id, result.Value.Id);
        Assert.Equal(_testPdfReportDto.Name, result.Value.Name);
        Assert.Equal(_testPdfReportDto.BlobName, result.Value.BlobName);

        _mockPdfService.Verify(x => x.UploadPdfAsync(It.IsAny<IFormFile>(), It.IsAny<string>()), Times.Once);
        _mockRepositoryWrapper.Verify(x => x.PdfReportRepository.CreateAsync(It.IsAny<PdfReport>()), Times.Once);
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Once);
        _mockMapper.Verify(x => x.Map<PdfReportDto>(It.IsAny<PdfReport>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NullFile_ShouldReturnValidationError()
    {
        // Arrange
        var command = new CreatePdfReportCommand(new CreatePdfReportDto { File = null! });

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        _mockPdfService.Verify(x => x.UploadPdfAsync(It.IsAny<IFormFile>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ShouldReturnFailure()
    {
        // Arrange
        _mockPdfService.Setup(x => x.UploadPdfAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
            .ReturnsAsync("abc123.pdf");

        _mockReorderService.Setup(x => x.GetNextDisplayOrderAsync<PdfReport>(It.IsAny<System.Linq.Expressions.Expression<System.Func<PdfReport, bool>>>()))
            .ReturnsAsync(1);

        _mockRepositoryWrapper.Setup(x => x.PdfReportRepository.CreateAsync(It.IsAny<PdfReport>()))
            .ReturnsAsync(_testPdfReport);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(0);

        _mockRepositoryWrapper.Setup(x => x.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        var command = new CreatePdfReportCommand(new CreatePdfReportDto { File = _testFile });

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.FailedToCreateEntity(typeof(PdfReport)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_PdfServiceThrowsBlobStorageException_ShouldReturnFailure()
    {
        // Arrange
        _mockPdfService.Setup(x => x.UploadPdfAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
            .ThrowsAsync(new BlobFileSystemException("path", "Failed to save PDF file"));

        _mockRepositoryWrapper.Setup(x => x.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        var command = new CreatePdfReportCommand(new CreatePdfReportDto { File = _testFile });

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.BlobStorageError("Failed to save PDF file"), result.Errors[0].Message);
    }

    private CreatePdfReportHandler CreateHandler() =>
        new(
            _mockRepositoryWrapper.Object,
            _mockPdfService.Object,
            _validator,
            _mockMapper.Object,
            _mockReorderService.Object);
}
