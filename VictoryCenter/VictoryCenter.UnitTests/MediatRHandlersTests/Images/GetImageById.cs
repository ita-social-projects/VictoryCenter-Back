using AutoMapper;
using Moq;
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
        var command = new GetImageByIdQuery(id: 1);

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
        // Arrange
        var command = new GetImageByIdQuery(id: 123);

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
        Assert.Contains("image not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_BlobNameIsNull_ShouldReturnNotFound()
    {
        // Arrange
        var imageWithoutBlob = new Image
        {
            Id = 2,
            BlobName = null,
            MimeType = "image/png"
        };
        var command = new GetImageByIdQuery(id: 2);

        _mockRepositoryWrapper.Setup(x => x.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync(imageWithoutBlob);

        var handler = new GetImageByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _blobservice.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("image not found", result.Errors[0].Message);
    }
}
