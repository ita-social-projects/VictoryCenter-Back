using AutoMapper;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Queries.Images.GetById;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Images;

public class GetImageByIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IBlobService> _blobservice;
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

    public GetImageByIdHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _blobservice = new Mock<IBlobService>();
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReturnImageDto()
    {
        // Arrange
        var command = new GetImageByIdQuery(Id: 1);

        _mockRepositoryWrapper.Setup(x => x.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync(_testImage);

        _mockMapper.Setup(x => x.Map<ImageDTO>(It.IsAny<Image>()))
            .Returns(_testImageDto);

        var handler = new GetImageByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _blobservice.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_testImageDto.Id, result.Value.Id);
        Assert.Equal(_testImageDto.BlobName, result.Value.BlobName);
        Assert.Equal(_testImageDto.MimeType, result.Value.MimeType);
        Assert.Equal(_testImageDto.Base64, result.Value.Base64);
    }

    [Fact]
    public async Task Handle_ImageNotFound_ShouldReturnNotFound()
    {
        const long id = 123;

        // Arrange
        var command = new GetImageByIdQuery(Id: id);

        _mockRepositoryWrapper.Setup(x => x.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync((Image?)null);

        var handler = new GetImageByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _blobservice.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(ImageConstants.ImageNotFound(id), result.Errors[0].Message, StringComparison.OrdinalIgnoreCase);
        _mockRepositoryWrapper.Verify(x => x.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WrongImageName_ShouldReturnNotFound()
    {
        const long id = 1;

        // Arrange
        var command = new GetImageByIdQuery(Id: id);
        var mockImage = new Image()
        {
            Id = id,
            Base64 = "dGVzdA==",
            BlobName = "",
            MimeType = "image/png",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepositoryWrapper.Setup(x => x.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync(mockImage);

        var handler = new GetImageByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _blobservice.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(ImageConstants.ImageDataNotAvailable, result.Errors[0].Message, StringComparison.OrdinalIgnoreCase);
        _mockRepositoryWrapper.Verify(x => x.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()), Times.Once);
    }
}
