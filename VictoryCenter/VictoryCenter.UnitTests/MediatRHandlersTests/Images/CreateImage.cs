using System.Transactions;
using AutoMapper;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Images.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Exceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.BLL.Validators.Images;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Images;

public class CreateImageHandlerTests
{
    private readonly Mock<IBlobService> _mockBlobService;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly IValidator<CreateImageCommand> _validator;

    private readonly CreateImageDTO _testCreateImageDto = new()
    {
        Base64 = "dGVzdA==", // "test" in base64
        MimeType = "image/png"
    };

    private readonly Image _testImage = new()
    {
        Id = 1,
        BlobName = "testblob",
        MimeType = "image/png"
    };

    private readonly ImageDTO _testImageDto = new()
    {
        Id = 1,
        BlobName = "testblob",
        MimeType = "image/png",
        Url = "dGVzdA=="
    };

    public CreateImageHandlerTests()
    {
        _validator = new CreateImageValidator();
        _mockBlobService = new Mock<IBlobService>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateImageAndReturnDto()
    {
        // Arrange
        var fileName = "testblob";
        var fileWithExtension = "testblob.png";
        _mockBlobService.Setup(x => x.SaveFileInStorageAsync(_testCreateImageDto.Base64, It.IsAny<string>(), _testCreateImageDto.MimeType))
            .ReturnsAsync(fileWithExtension);

        _mockMapper.Setup(x => x.Map<Image>(It.IsAny<CreateImageDTO>()))
            .Returns(_testImage);

        _mockRepositoryWrapper.Setup(x => x.ImageRepository.CreateAsync(It.IsAny<Image>()))
            .ReturnsAsync(_testImage);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _mockRepositoryWrapper.Setup(repositoryWrapper => repositoryWrapper.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        _mockMapper.Setup(x => x.Map<ImageDTO>(It.IsAny<Image>()))
            .Returns(_testImageDto);

        var handler = new CreateImageHandler(
            _mockBlobService.Object,
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _validator);

        var command = new CreateImageCommand(_testCreateImageDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_testImageDto.Id, result.Value.Id);
        Assert.Equal(_testImageDto.BlobName, result.Value.BlobName);
        Assert.Equal(_testImageDto.MimeType, result.Value.MimeType);
        Assert.Equal(_testImageDto.Url, result.Value.Url);
        _mockBlobService.Verify(x => x.SaveFileInStorageAsync(_testCreateImageDto.Base64, It.IsAny<string>(), _testCreateImageDto.MimeType), Times.Once);
        _mockRepositoryWrapper.Verify(x => x.ImageRepository.CreateAsync(It.IsAny<Image>()), Times.Once);
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Once);
        _mockMapper.Verify(x => x.Map<Image>(It.IsAny<CreateImageDTO>()), Times.Once);
        _mockMapper.Verify(x => x.Map<ImageDTO>(It.IsAny<Image>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidRequest_ShouldReturnValidationError()
    {
        // Arrange: Create invalid DTO (empty Base64)
        var invalidDto = new CreateImageDTO { Base64 = "", MimeType = "" };
        var command = new CreateImageCommand(invalidDto);

        var handler = new CreateImageHandler(
            _mockBlobService.Object,
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _validator);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Message.Contains("cannot be null") || e.Message.Contains("required"));
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ShouldReturnFailure()
    {
        // Arrange
        _mockBlobService.Setup(x => x.SaveFileInStorageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("testblob.png");

        _mockMapper.Setup(x => x.Map<Image>(It.IsAny<CreateImageDTO>()))
            .Returns(_testImage);

        _mockRepositoryWrapper.Setup(x => x.ImageRepository.CreateAsync(It.IsAny<Image>()))
            .ReturnsAsync(_testImage);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(0);

        var handler = new CreateImageHandler(
            _mockBlobService.Object,
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _validator);

        var command = new CreateImageCommand(_testCreateImageDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(ImageConstants.FailToSaveImageInDatabase, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ThrowsIOException_ShouldReturnFileCreatingFail()
    {
        // Arrange
        _mockMapper.Setup(x => x.Map<Image>(It.IsAny<CreateImageDTO>())).Returns(_testImage);
        _mockRepositoryWrapper.Setup(x => x.ImageRepository.CreateAsync(It.IsAny<Image>())).ReturnsAsync(_testImage);
        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        _mockBlobService
            .Setup(x => x.SaveFileInStorageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new BlobFileSystemException("mockpath", ImageConstants.FailToSaveImageInStorage));

        var handler = new CreateImageHandler(
            _mockBlobService.Object,
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _validator);

        var command = new CreateImageCommand(_testCreateImageDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.BlobStorageError(ImageConstants.FailToSaveImageInStorage), result.Errors[0].Message);
    }
}
