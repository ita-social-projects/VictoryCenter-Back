using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.PdfReports.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.Hubs;
using VictoryCenter.BLL.Validators.PdfReports;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.PdfReports;

public class UpdatePdfReportHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly IValidator<UpdatePdfReportCommand> _validator;
    private readonly PdfReport _testPdfReport;
    private readonly PdfReportDto _testPdfReportDto;
    private readonly Mock<IHubContext<PdfReportsHub>> _mockHubContext;

    public UpdatePdfReportHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockHubContext = new Mock<IHubContext<PdfReportsHub>>();
        _validator = new UpdatePdfReportValidator();

        _testPdfReport = new PdfReport
        {
            Id = 1,
            Name = "Old Report Name",
            BlobName = "blob-name-123.pdf",
            FileSizeBytes = 1024000,
            Priority = 1,
            LanguageId = 1,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _testPdfReportDto = new PdfReportDto
        {
            Id = 1,
            Name = "Old Report Name",
            BlobName = "blob-name-123.pdf",
            FileSizeBytes = 1024000,
            Priority = 1,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    [Fact]
    public async Task Handle_ValidRequest_WithNameChange_ShouldUpdateAndReturnDto()
    {
        // Arrange
        var newName = "New Report Name";
        var command = new UpdatePdfReportCommand(1, newName);

        _mockRepositoryWrapper
            .Setup(x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync(_testPdfReport);

        _mockRepositoryWrapper
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        var updatedDto = new PdfReportDto
        {
            Id = 1,
            Name = newName,
            BlobName = "blob-name-123.pdf",
            FileSizeBytes = 1024000,
            Priority = 1,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _mockMapper
            .Setup(x => x.Map<PdfReportDto>(It.IsAny<PdfReport>()))
            .Returns(updatedDto);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(newName, result.Value.Name);
        Assert.Equal(1, result.Value.Id);

        _mockRepositoryWrapper.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
        _mockMapper.Verify(
            x => x.Map<PdfReportDto>(It.IsAny<PdfReport>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidRequest_NoNameChange_ShouldNotSaveAndReturnDto()
    {
        // Arrange
        var command = new UpdatePdfReportCommand(1, _testPdfReport.Name);

        _mockRepositoryWrapper
            .Setup(x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync(_testPdfReport);

        _mockMapper
            .Setup(x => x.Map<PdfReportDto>(It.IsAny<PdfReport>()))
            .Returns(_testPdfReportDto);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_testPdfReport.Name, result.Value.Name);

        _mockRepositoryWrapper.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
        _mockMapper.Verify(
            x => x.Map<PdfReportDto>(It.IsAny<PdfReport>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidRequest_WithMultipleSpaces_ShouldNormalizeAndUpdate()
    {
        // Arrange
        var nameWithMultipleSpaces = "Report   With    Multiple    Spaces";
        var normalizedName = "Report With Multiple Spaces";
        var command = new UpdatePdfReportCommand(1, nameWithMultipleSpaces);

        var testReport = new PdfReport
        {
            Id = 1,
            Name = "Old Name",
            BlobName = "blob-name-123.pdf",
            FileSizeBytes = 1024000,
            Priority = 1,
            LanguageId = 1,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _mockRepositoryWrapper
            .Setup(x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync(testReport);

        _mockRepositoryWrapper
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        var updatedDto = new PdfReportDto
        {
            Id = 1,
            Name = normalizedName,
            BlobName = "blob-name-123.pdf",
            FileSizeBytes = 1024000,
            Priority = 1,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _mockMapper
            .Setup(x => x.Map<PdfReportDto>(It.IsAny<PdfReport>()))
            .Returns(updatedDto);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(normalizedName, result.Value.Name);
        Assert.DoesNotContain("   ", result.Value.Name);
    }

    [Fact]
    public async Task Handle_ValidRequest_WithWhitespace_ShouldTrimAndUpdate()
    {
        // Arrange
        var nameWithWhitespace = "  Report Name  ";
        var trimmedName = "Report Name";
        var command = new UpdatePdfReportCommand(1, nameWithWhitespace);

        var testReport = new PdfReport
        {
            Id = 1,
            Name = "Old Name",
            BlobName = "blob-name-123.pdf",
            FileSizeBytes = 1024000,
            Priority = 1,
            LanguageId = 1,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _mockRepositoryWrapper
            .Setup(x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync(testReport);

        _mockRepositoryWrapper
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        var updatedDto = new PdfReportDto
        {
            Id = 1,
            Name = trimmedName,
            BlobName = "blob-name-123.pdf",
            FileSizeBytes = 1024000,
            Priority = 1,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _mockMapper
            .Setup(x => x.Map<PdfReportDto>(It.IsAny<PdfReport>()))
            .Returns(updatedDto);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(trimmedName, result.Value.Name);
    }

    [Fact]
    public async Task Handle_WhitespaceOnlyName_ReturnsValidationError()
    {
        var command = new UpdatePdfReportCommand(1, "   ");
        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(
            PdfReportConstants.NameRequiredErrorMessage,
            result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ReportNotFound_ReturnsFailResult()
    {
        // Arrange
        var command = new UpdatePdfReportCommand(999, "New Name");

        _mockRepositoryWrapper
            .Setup(x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync((PdfReport?)null);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.NotFound(999, typeof(PdfReport)),
            result.Errors.Select(e => e.Message));

        _mockRepositoryWrapper.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
        _mockMapper.Verify(
            x => x.Map<PdfReportDto>(It.IsAny<PdfReport>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_SaveChangesReturnsZero_ReturnsFailResult()
    {
        // Arrange
        var newName = "New Report Name";
        var command = new UpdatePdfReportCommand(1, newName);

        _mockRepositoryWrapper
            .Setup(x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync(_testPdfReport);

        _mockRepositoryWrapper
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(0);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(PdfReport)),
            result.Errors.Select(e => e.Message));

        _mockRepositoryWrapper.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
        _mockMapper.Verify(
            x => x.Map<PdfReportDto>(It.IsAny<PdfReport>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_DbUpdateException_ReturnsFailResult()
    {
        // Arrange
        var newName = "New Report Name";
        var command = new UpdatePdfReportCommand(1, newName);

        _mockRepositoryWrapper
            .Setup(x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync(_testPdfReport);

        _mockRepositoryWrapper
            .Setup(x => x.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(PdfReport)),
            result.Errors.Select(e => e.Message));

        _mockMapper.Verify(
            x => x.Map<PdfReportDto>(It.IsAny<PdfReport>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidId_ReturnsValidationError()
    {
        // Arrange
        var command = new UpdatePdfReportCommand(-1, "New Name");
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        _mockRepositoryWrapper.Verify(
            x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_EmptyName_ReturnsValidationError()
    {
        // Arrange
        var command = new UpdatePdfReportCommand(1, "");
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        _mockRepositoryWrapper.Verify(
            x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_NameTooShort_ReturnsValidationError()
    {
        // Arrange
        var command = new UpdatePdfReportCommand(1, "A");
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        _mockRepositoryWrapper.Verify(
            x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_NameTooShortAfterNormalization_ReturnsValidationError()
    {
        // Arrange
        var command = new UpdatePdfReportCommand(1, " A  ");
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            PdfReportConstants.NameMinLengthErrorMessage,
            result.Errors.Select(e => e.Message));
        _mockRepositoryWrapper.Verify(
            x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_NameTooLongAfterNormalization_ReturnsValidationError()
    {
        // Arrange
        var longName = new string('a', PdfReportConstants.NameMaxLength + 1);
        var command = new UpdatePdfReportCommand(1, longName);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            PdfReportConstants.NameMaxLengthErrorMessage,
            result.Errors.Select(e => e.Message));
        _mockRepositoryWrapper.Verify(
            x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()),
            Times.Never);
    }

    private UpdatePdfReportHandler CreateHandler() =>
        new(_mockRepositoryWrapper.Object, _validator, _mockMapper.Object, _mockHubContext.Object);
}
