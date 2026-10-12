using AutoMapper;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Images.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Services.ImageValidation;
using VictoryCenter.BLL.Validators.Images;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.UnitTests.Utils.Images;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Images;

public class UpdateImageHandlerTests
{
    private const string PreviousBlobName = "previous-blob";
    private const string PreviousMimeType = "image/jpeg";

    private static readonly DateTimeOffset TestNow = new(2025, 7, 16, 14, 30, 0, TimeSpan.Zero);

    private readonly Mock<IBlobService> _mockBlobService = new();
    private readonly Mock<ILogger<UpdateImageHandler>> _mockLogger = new();
    private readonly Mock<IMapper> _mockMapper = new();
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper = new();
    private readonly Mock<TimeProvider> _mockTimeProvider = new();
    private readonly IValidator<UpdateImageCommand> _validator = new UpdateImageValidator(new ImageContentValidator());

    private readonly UpdateImageDto _testUpdateImageDto = new()
    {
        Base64 = ImageTestData.CreateBase64("image/png"),
        MimeType = "image/png"
    };

    private readonly Image _testImage = new()
    {
        Id = 1,
        BlobName = PreviousBlobName,
        MimeType = PreviousMimeType,
        CreatedAt = TestNow
    };

    public UpdateImageHandlerTests()
    {
        _mockTimeProvider.Setup(provider => provider.GetUtcNow()).Returns(TestNow);
        _mockMapper
            .Setup(mapper => mapper.Map<Image, ImageDto>(It.IsAny<Image>()))
            .Returns((Image image) => new ImageDto
            {
                Id = image.Id,
                BlobName = image.BlobName,
                MimeType = image.MimeType,
                CreatedAt = image.CreatedAt,
                UpdatedAt = image.UpdatedAt
            });
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldPersistReplacementBeforeDeletingPreviousBlob()
    {
        var operations = new List<string>();
        SetupExistingImage();
        _mockBlobService
            .Setup(service => service.SaveFileInStorageAsync(
                _testUpdateImageDto.Base64!,
                It.IsAny<string>(),
                _testUpdateImageDto.MimeType!))
            .Callback(() => operations.Add("save-new"))
            .ReturnsAsync((string _, string name, string _) => $"{name}.png");
        _mockRepositoryWrapper
            .Setup(wrapper => wrapper.SaveChangesAsync())
            .Callback(() => operations.Add("update-database"))
            .ReturnsAsync(1);
        _mockBlobService
            .Setup(service => service.DeleteFileInStorage(PreviousBlobName, PreviousMimeType))
            .Callback(() => operations.Add("delete-old"));

        var result = await CreateHandler().Handle(
            new UpdateImageCommand(_testUpdateImageDto, _testImage.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(PreviousBlobName, result.Value.BlobName);
        Assert.Equal(_testUpdateImageDto.MimeType, result.Value.MimeType);
        Assert.Equal(TestNow, result.Value.UpdatedAt);
        Assert.Equal(["save-new", "update-database", "delete-old"], operations);
        _mockBlobService.Verify(
            service => service.SaveFileInStorageAsync(
                _testUpdateImageDto.Base64!,
                result.Value.BlobName,
                _testUpdateImageDto.MimeType!),
            Times.Once);
        _mockBlobService.Verify(
            service => service.DeleteFileInStorage(PreviousBlobName, PreviousMimeType),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ImageNotFound_ShouldNotWriteBlob()
    {
        const long id = 123;
        _mockRepositoryWrapper
            .Setup(wrapper => wrapper.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync((Image?)null);

        var result = await CreateHandler().Handle(
            new UpdateImageCommand(_testUpdateImageDto, id),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(ErrorMessagesConstants.NotFound(id, typeof(Image)), result.Errors[0].Message);
        _mockBlobService.Verify(
            service => service.SaveFileInStorageAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_NullDto_ShouldReturnValidationErrorWithoutWritingBlob()
    {
        Result<ImageDto> result = await CreateHandler().Handle(
            new UpdateImageCommand(null!, 1),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(
            ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateImageCommand.UpdateImageDto)),
            result.Errors.Select(error => error.Message));
        _mockBlobService.Verify(
            service => service.SaveFileInStorageAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ReplacementBlobWriteFails_ShouldPreserveDatabaseAndPreviousBlob()
    {
        SetupExistingImage();
        _mockBlobService
            .Setup(service => service.SaveFileInStorageAsync(
                _testUpdateImageDto.Base64!,
                It.IsAny<string>(),
                _testUpdateImageDto.MimeType!))
            .ThrowsAsync(new ImageProcessingException("replacement", "Storage failure"));

        var result = await CreateHandler().Handle(
            new UpdateImageCommand(_testUpdateImageDto, _testImage.Id),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(PreviousBlobName, _testImage.BlobName);
        Assert.Equal(PreviousMimeType, _testImage.MimeType);
        _mockRepositoryWrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Never);
        _mockBlobService.Verify(
            service => service.DeleteFileInStorage(PreviousBlobName, PreviousMimeType),
            Times.Never);
    }

    [Fact]
    public async Task Handle_DatabaseUpdateFails_ShouldDeleteReplacementAndPreservePreviousBlob()
    {
        string? replacementBlobName = null;
        SetupExistingImage();
        _mockRepositoryWrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(0);
        _mockBlobService
            .Setup(service => service.SaveFileInStorageAsync(
                _testUpdateImageDto.Base64!,
                It.IsAny<string>(),
                _testUpdateImageDto.MimeType!))
            .Callback<string, string, string>((_, name, _) => replacementBlobName = name)
            .ReturnsAsync((string _, string name, string _) => $"{name}.png");

        var result = await CreateHandler().Handle(
            new UpdateImageCommand(_testUpdateImageDto, _testImage.Id),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.NotNull(replacementBlobName);
        _mockBlobService.Verify(
            service => service.DeleteFileInStorage(replacementBlobName, _testUpdateImageDto.MimeType!),
            Times.Once);
        _mockBlobService.Verify(
            service => service.DeleteFileInStorage(PreviousBlobName, PreviousMimeType),
            Times.Never);
    }

    [Fact]
    public async Task Handle_DatabaseUpdateThrows_ShouldDeleteReplacementAndPreservePreviousBlob()
    {
        string? replacementBlobName = null;
        SetupExistingImage();
        _mockRepositoryWrapper
            .Setup(wrapper => wrapper.SaveChangesAsync())
            .ThrowsAsync(new InvalidOperationException("Database update failed"));
        _mockBlobService
            .Setup(service => service.SaveFileInStorageAsync(
                _testUpdateImageDto.Base64!,
                It.IsAny<string>(),
                _testUpdateImageDto.MimeType!))
            .Callback<string, string, string>((_, name, _) => replacementBlobName = name)
            .ReturnsAsync((string _, string name, string _) => $"{name}.png");
        await Assert.ThrowsAsync<InvalidOperationException>(() => CreateHandler().Handle(
            new UpdateImageCommand(_testUpdateImageDto, _testImage.Id),
            CancellationToken.None));

        Assert.NotNull(replacementBlobName);
        _mockBlobService.Verify(
            service => service.DeleteFileInStorage(replacementBlobName, _testUpdateImageDto.MimeType!),
            Times.Once);
        _mockBlobService.Verify(
            service => service.DeleteFileInStorage(PreviousBlobName, PreviousMimeType),
            Times.Never);
    }

    [Fact]
    public async Task Handle_PreviousBlobCleanupFails_ShouldKeepSuccessfulReplacement()
    {
        SetupExistingImage();
        _mockRepositoryWrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(1);
        _mockBlobService
            .Setup(service => service.DeleteFileInStorage(PreviousBlobName, PreviousMimeType))
            .Throws(new ImageProcessingException(PreviousBlobName, "Cleanup failed"));

        var result = await CreateHandler().Handle(
            new UpdateImageCommand(_testUpdateImageDto, _testImage.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(PreviousBlobName, result.Value.BlobName);
        _mockRepositoryWrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Once);
    }

    private void SetupExistingImage()
    {
        _mockRepositoryWrapper
            .Setup(wrapper => wrapper.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync(_testImage);
    }

    private UpdateImageHandler CreateHandler()
    {
        return new UpdateImageHandler(
            _mockMapper.Object,
            _mockRepositoryWrapper.Object,
            _validator,
            _mockBlobService.Object,
            _mockTimeProvider.Object,
            _mockLogger.Object);
    }
}
