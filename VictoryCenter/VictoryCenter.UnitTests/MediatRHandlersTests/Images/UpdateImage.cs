using AutoMapper;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Images.Update;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Images;

public class UpdateImageHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IBlobService> _mockBlobService;
    private readonly IValidator<UpdateImageCommand> _validator;

    private readonly UpdateImageDTO _testUpdateImageDto = new()
    {
        Base64 = "dGVzdA==",
        MimeType = "image/png"
    };

    private readonly Image _testImage = new()
    {
        Id = 1,
        BlobName = "testblob.png",
        MimeType = "image/png"
    };

    private readonly ImageDTO _testImageDto = new()
    {
        Id = 1,
        BlobName = "testblob.png",
        MimeType = "image/png",
        Base64 = "dGVzdA=="
    };

    public UpdateImageHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockBlobService = new Mock<IBlobService>();
        _validator = new InlineValidator<UpdateImageCommand>();
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldUpdateImageAndReturnDto()
    {
        var command = new UpdateImageCommand(_testUpdateImageDto, 1);

        _mockRepositoryWrapper.Setup(x => x.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync(_testImage);

        _mockBlobService.Setup(x => x.UpdateFileInStorage(
                _testImage.BlobName,
                _testImage.MimeType,
                _testUpdateImageDto.Base64,
                _testImage.BlobName,
                _testUpdateImageDto.MimeType))
            .Returns(_testImage.BlobName);

        _mockMapper.Setup(x => x.Map<UpdateImageDTO, Image>(It.IsAny<UpdateImageDTO>()))
            .Returns(_testImage);

        _mockMapper.Setup(x => x.Map<Image, ImageDTO>(It.IsAny<Image>()))
            .Returns(_testImageDto);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        var handler = new UpdateImageHandler(
            _mockMapper.Object,
            _mockRepositoryWrapper.Object,
            _validator,
            _mockBlobService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_testImageDto.Id, result.Value.Id);
        Assert.Equal(_testImageDto.BlobName, result.Value.BlobName);
        Assert.Equal(_testImageDto.MimeType, result.Value.MimeType);
        Assert.Equal(_testImageDto.Base64, result.Value.Base64);
        _mockBlobService.Verify(
            x => x.UpdateFileInStorage(
            _testImage.BlobName,
            _testImage.MimeType,
            _testUpdateImageDto.Base64,
            _testImage.BlobName,
            _testUpdateImageDto.MimeType), Times.Once);
    }

    [Fact]
    public async Task Handle_ImageNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var command = new UpdateImageCommand(_testUpdateImageDto, 123);

        _mockRepositoryWrapper.Setup(x => x.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync((Image?)null);

        var handler = new UpdateImageHandler(
            _mockMapper.Object,
            _mockRepositoryWrapper.Object,
            _validator,
            _mockBlobService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Not found", result.Errors[0].Message);
        _mockBlobService.Verify(
            x => x.UpdateFileInStorage(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateImageCommand(_testUpdateImageDto, 1);

        _mockRepositoryWrapper.Setup(x => x.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync(_testImage);

        _mockBlobService.Setup(x => x.UpdateFileInStorage(
                _testImage.BlobName,
                _testImage.MimeType,
                _testUpdateImageDto.Base64,
                _testImage.BlobName,
                _testUpdateImageDto.MimeType))
            .Returns(_testImage.BlobName);

        _mockMapper.Setup(x => x.Map<UpdateImageDTO, Image>(It.IsAny<UpdateImageDTO>()))
            .Returns(_testImage);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(0);

        var handler = new UpdateImageHandler(
            _mockMapper.Object,
            _mockRepositoryWrapper.Object,
            _validator,
            _mockBlobService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Failed to update image", result.Errors[0].Message);
    }
}
