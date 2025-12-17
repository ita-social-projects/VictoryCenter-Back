using FluentResults;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
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
    public async Task ValidateAndGetImageAsync_ImageIdIsNull_IsSuccess()
    {
        Result<Image?> result =
            await ImageValidationHelper.ValidateAndGetImageAsync(_wrapperMock.Object, null);

        Assert.True(result.IsSuccess);

        _imageRepoMock.Verify(
            r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()),
            Times.Never);
    }

    [Fact]
    public async Task ValidateAndGetImageAsync_ImageIdIsNull_ValueIsNull()
    {
        Result<Image?> result =
            await ImageValidationHelper.ValidateAndGetImageAsync(_wrapperMock.Object, null);

        Assert.Null(result.ValueOrDefault);
    }

    [Fact]
    public async Task ValidateAndGetImageAsync_ImageFound_IsSuccess()
    {
        const long imageId = 10;
        var image = MakeImage(imageId);

        _imageRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync(image);

        Result<Image?> result =
            await ImageValidationHelper.ValidateAndGetImageAsync(_wrapperMock.Object, imageId);

        Assert.True(result.IsSuccess);

        _imageRepoMock.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.Is<QueryOptions<Image>>(opts => VerifyQueryOptions(opts, imageId))),
            Times.Once);
    }

    [Fact]
    public async Task ValidateAndGetImageAsync_ImageFound_ReturnsSameImageInstance()
    {
        const long imageId = 10;
        var image = MakeImage(imageId);

        _imageRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync(image);

        Result<Image?> result =
            await ImageValidationHelper.ValidateAndGetImageAsync(_wrapperMock.Object, imageId);

        Assert.Same(image, result.ValueOrDefault);
    }

    [Fact]
    public async Task ValidateAndGetImageAsync_ImageNotFound_IsFailed()
    {
        const long imageId = 20;

        _imageRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync((Image?)null);

        Result<Image?> result =
            await ImageValidationHelper.ValidateAndGetImageAsync(_wrapperMock.Object, imageId);

        Assert.True(result.IsFailed);

        _imageRepoMock.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.Is<QueryOptions<Image>>(opts => VerifyQueryOptions(opts, imageId))),
            Times.Once);
    }

    [Fact]
    public async Task ValidateAndGetImageAsync_ImageNotFound_ReturnsExpectedErrorMessage()
    {
        const long imageId = 20;

        _imageRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync((Image?)null);

        Result<Image?> result =
            await ImageValidationHelper.ValidateAndGetImageAsync(_wrapperMock.Object, imageId);

        Assert.Equal(
            ErrorMessagesConstants.NotFound(imageId, typeof(Image)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task ValidateAndGetImageAsync_RepositoryThrowsBlobStorageException_IsFailed()
    {
        _imageRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ThrowsAsync(MakeBlobStorageException());

        Result<Image?> result =
            await ImageValidationHelper.ValidateAndGetImageAsync(_wrapperMock.Object, 1);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task ValidateAndGetImageAsync_RepositoryThrowsBlobStorageException_ReturnsExpectedErrorMessage()
    {
        _imageRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ThrowsAsync(MakeBlobStorageException());

        Result<Image?> result =
            await ImageValidationHelper.ValidateAndGetImageAsync(_wrapperMock.Object, 1);

        Assert.Equal(ErrorMessagesConstants.FailedToRetrieveImage(), result.Errors[0].Message);
    }

    [Fact]
    public async Task ValidateAndGetImagesByIdsAsync_AllIdsInvalid_ReturnsEmptyDictionary()
    {
        Result<IReadOnlyDictionary<long, Image>> result =
            await ImageValidationHelper.ValidateAndGetImagesByIdsAsync(
                _wrapperMock.Object,
                [-2, -1, 0]);

        Assert.Empty(result.Value);

        _imageRepoMock.Verify(
            r => r.GetAllAsync(It.IsAny<QueryOptions<Image>>()),
            Times.Never);
    }

    [Fact]
    public async Task ValidateAndGetImagesByIdsAsync_QueryOptionsFilterMatchesDistinctPositiveIds()
    {
        QueryOptions<Image>? captured = null;

        _imageRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .Callback<QueryOptions<Image>>(opts => captured = opts)
            .ReturnsAsync([MakeImage(1), MakeImage(2)]);

        await ImageValidationHelper.ValidateAndGetImagesByIdsAsync(
            _wrapperMock.Object,
            [0, 1, 1, 2, -5]);

        Assert.True(VerifyGetAllQueryOptions(captured, [1, 2]));
    }

    [Fact]
    public async Task ValidateAndGetImagesByIdsAsync_AllFound_IsSuccess()
    {
        _imageRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync([MakeImage(1), MakeImage(2)]);

        Result<IReadOnlyDictionary<long, Image>> result =
            await ImageValidationHelper.ValidateAndGetImagesByIdsAsync(
                _wrapperMock.Object,
                [1, 2]);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ValidateAndGetImagesByIdsAsync_AllFound_ReturnsDictionaryCount()
    {
        _imageRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync([MakeImage(1), MakeImage(2)]);

        Result<IReadOnlyDictionary<long, Image>> result =
            await ImageValidationHelper.ValidateAndGetImagesByIdsAsync(
                _wrapperMock.Object,
                [1, 2]);

        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public async Task ValidateAndGetImagesByIdsAsync_SomeMissing_IsFailed()
    {
        _imageRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync([MakeImage(1)]);

        Result<IReadOnlyDictionary<long, Image>> result =
            await ImageValidationHelper.ValidateAndGetImagesByIdsAsync(
                _wrapperMock.Object,
                [1, 2]);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task ValidateAndGetImagesByIdsAsync_SomeMissing_ReturnsExpectedErrorMessage()
    {
        _imageRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync([MakeImage(1)]);

        Result<IReadOnlyDictionary<long, Image>> result =
            await ImageValidationHelper.ValidateAndGetImagesByIdsAsync(
                _wrapperMock.Object,
                [1, 2]);

        Assert.Equal(
            ErrorMessagesConstants.NotFound([2], typeof(Image)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task ValidateAndGetImagesByIdsAsync_RepositoryThrowsBlobStorageException_IsFailed()
    {
        _imageRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .ThrowsAsync(MakeBlobStorageException());

        Result<IReadOnlyDictionary<long, Image>> result =
            await ImageValidationHelper.ValidateAndGetImagesByIdsAsync(
                _wrapperMock.Object,
                [1, 2]);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task ValidateAndGetImagesByIdsAsync_RepositoryThrowsBlobStorageException_ReturnsExpectedErrorMessage()
    {
        _imageRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .ThrowsAsync(MakeBlobStorageException());

        Result<IReadOnlyDictionary<long, Image>> result =
            await ImageValidationHelper.ValidateAndGetImagesByIdsAsync(
                _wrapperMock.Object,
                [1, 2]);

        Assert.Equal(ErrorMessagesConstants.FailedToRetrieveImage(), result.Errors[0].Message);
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

    private static bool VerifyGetAllQueryOptions(QueryOptions<Image>? opts, IEnumerable<long> expectedIds)
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
        var ids = expectedIds.ToList();

        foreach (var id in ids)
        {
            if (!filter(new Image { Id = id }))
            {
                return false;
            }
        }

        if (filter(new Image { Id = 999999 }))
        {
            return false;
        }

        return true;
    }

    private static BlobStorageException MakeBlobStorageException()
    {
        return new Mock<BlobStorageException>("test").Object;
    }
}
