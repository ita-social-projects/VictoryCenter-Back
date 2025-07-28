using System.Transactions;
using Moq;
using VictoryCenter.BLL.Commands.Images.Delete;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Images;

public class DeleteImageHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IBlobService> _mockBlobService;

    private readonly Image _testImage = new()
    {
        Id = 1L,
        BlobName = "testblob.png",
        MimeType = "image/png"
    };

    public DeleteImageHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockBlobService = new Mock<IBlobService>();
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldDeleteImageAndFile()
    {
        // Arrange
        _mockRepositoryWrapper.Setup(x => x.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync(_testImage);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _mockRepositoryWrapper.Setup(repositoryWrapper => repositoryWrapper.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        var handler = new DeleteImageHandler(_mockRepositoryWrapper.Object, _mockBlobService.Object);

        // Fix: Use the constructor to initialize the required parameter 'Id'
        var command = new DeleteImageCommand(_testImage.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_testImage.Id, result.Value);
        _mockBlobService.Verify(x => x.DeleteFileInStorage(_testImage.BlobName, _testImage.MimeType), Times.Once);
        _mockRepositoryWrapper.Verify(x => x.ImageRepository.Delete(_testImage), Times.Once);
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ImageNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _mockRepositoryWrapper.Setup(x => x.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync((Image?)null);

        var handler = new DeleteImageHandler(_mockRepositoryWrapper.Object, _mockBlobService.Object);

        // Fix: Use the constructor to initialize the required parameter 'Id'
        var command = new DeleteImageCommand(123L);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Errors[0].Message);
        _mockBlobService.Verify(x => x.DeleteFileInStorage(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockRepositoryWrapper.Verify(x => x.ImageRepository.Delete(It.IsAny<Image>()), Times.Never);
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ShouldReturnFailure()
    {
        // Arrange
        _mockRepositoryWrapper.Setup(x => x.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync(_testImage);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(0);

        _mockRepositoryWrapper.Setup(repositoryWrapper => repositoryWrapper.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        var handler = new DeleteImageHandler(_mockRepositoryWrapper.Object, _mockBlobService.Object);

        // Fix: Use the constructor to initialize the required parameter 'Id'
        var command = new DeleteImageCommand(_testImage.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Failed to delete image", result.Errors[0].Message);
        _mockRepositoryWrapper.Verify(x => x.ImageRepository.Delete(_testImage), Times.Once);
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
