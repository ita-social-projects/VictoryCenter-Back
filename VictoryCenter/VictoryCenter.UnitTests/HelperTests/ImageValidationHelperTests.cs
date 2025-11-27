using FluentResults;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.HelperTests;

public class ImageValidationHelperTests
{
    private readonly Mock<IRepositoryWrapper> _wrapperMock;
    private readonly Mock<IImageRepository> _imageRepoMock;

    public ImageValidationHelperTests()
    {
        _wrapperMock = new Mock<IRepositoryWrapper>();
        _imageRepoMock = new Mock<IImageRepository>();

        _wrapperMock
            .SetupGet(w => w.ImageRepository)
            .Returns(_imageRepoMock.Object);
    }

    [Fact]
    public async Task ValidateAndGetImageAsync_ImageIdIsNull_ReturnsOkWithNull_AndDoesNotCallRepository()
    {
        // Act
        Result<Image?> result =
            await ImageValidationHelper.ValidateAndGetImageAsync(_wrapperMock.Object, null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.ValueOrDefault);

        _imageRepoMock.Verify(
            r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()),
            Times.Never);
    }

    [Fact]
    public async Task ValidateAndGetImageAsync_ImageFound_ReturnsOkWithImage()
    {
        // Arrange
        const long imageId = 10;
        var image = MakeImage(imageId);

        _imageRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync(image);

        // Act
        Result<Image?> result =
            await ImageValidationHelper.ValidateAndGetImageAsync(_wrapperMock.Object, imageId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Same(image, result.ValueOrDefault);

        _imageRepoMock.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.Is<QueryOptions<Image>>(opts => VerifyQueryOptions(opts, imageId))),
            Times.Once);
    }

    [Fact]
    public async Task ValidateAndGetImageAsync_ImageNotFound_ReturnsFailedWithExpectedError()
    {
        // Arrange
        const long imageId = 20;

        _imageRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync((Image?)null);

        var expectedMessage = ErrorMessagesConstants.NotFound(
            imageId.ToString(),
            typeof(Image));

        // Act
        Result<Image?> result =
            await ImageValidationHelper.ValidateAndGetImageAsync(_wrapperMock.Object, imageId);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Null(result.ValueOrDefault);
        Assert.Single(result.Errors);
        Assert.Equal(expectedMessage, result.Errors[0].Message);

        _imageRepoMock.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.Is<QueryOptions<Image>>(opts => VerifyQueryOptions(opts, imageId))),
            Times.Once);
    }

    private static Image MakeImage(long id)
    {
        return new Image
        {
            Id = id,
            CreatedAt = DateTimeOffset.UtcNow,
            BlobName = "blob",
            MimeType = "image/png",
            Url = "https://example.com/image.png"
        };
    }

    private static bool VerifyQueryOptions(QueryOptions<Image>? opts, long expectedId)
    {
        if (opts is null)
        {
            return false;
        }

        if (opts.AsNoTracking)
        {
            return false;
        }

        if (opts.Filter is null)
        {
            return false;
        }

        var filter = opts.Filter.Compile();
        var matchesExpectedId = filter(new Image { Id = expectedId });
        var rejectsOtherId = !filter(new Image { Id = expectedId + 1 });

        return matchesExpectedId && rejectsOtherId;
    }
}
