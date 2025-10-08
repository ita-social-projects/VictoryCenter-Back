using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Partners.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Partners;

public class DeletePartnerSectionTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepoWrapper;
    private readonly Mock<IReorderService> _mockReorderService;
    private readonly Mock<IBlobService> _mockBlobService;

    private readonly PartnerSection _existingSection = new()
    {
        Id = 1,
        Title = "Test Section",
        Priority = 1,
        Partners = new List<Partner>
        {
            new() { Id = 10, PartnersSectionId = 1, Image = new Image { Id = 100, BlobName = "blob1", MimeType = "image/png" } },
            new() { Id = 11, PartnersSectionId = 1, Image = new Image { Id = 101, BlobName = "blob2", MimeType = "image/jpeg" } }
        }
    };

    public DeletePartnerSectionTests()
    {
        _mockRepoWrapper = new Mock<IRepositoryWrapper>();
        _mockReorderService = new Mock<IReorderService>();
        _mockBlobService = new Mock<IBlobService>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public async Task Handle_SectionNotFound_ShouldReturnFailure(long sectionId)
    {
        // Arrange
        SetupRepositoryWrapper(); // Returns null by default
        var command = new DeletePartnersSectionCommand(sectionId);
        var handler = new DeletePartnersSectionHandler(_mockRepoWrapper.Object, _mockReorderService.Object, _mockBlobService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.NotFound(sectionId, typeof(PartnerSection)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_SectionExists_ShouldDeleteEntitiesAndFilesAndReturnOk()
    {
        // Arrange
        SetupRepositoryWrapper(_existingSection);
        SetupReorderService();
        var command = new DeletePartnersSectionCommand(_existingSection.Id);
        var handler = new DeletePartnersSectionHandler(_mockRepoWrapper.Object, _mockReorderService.Object, _mockBlobService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_existingSection.Id, result.Value);

        // Verify DB deletions
        _mockRepoWrapper.Verify(r => r.ImageRepository.DeleteRange(It.IsAny<IEnumerable<Image>>()), Times.Once);
        _mockRepoWrapper.Verify(r => r.PartnerRepository.DeleteRange(It.IsAny<IEnumerable<Partner>>()), Times.Once);
        _mockRepoWrapper.Verify(r => r.PartnerSectionsRepository.Delete(_existingSection), Times.Once);

        // Verify blob deletions
        _mockBlobService.Verify(b => b.DeleteFileInStorage(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(_existingSection.Partners.Count));

        // Verify reordering
        _mockReorderService.Verify(s => s.RenumberPriorityAsync<PartnerSection>(null), Times.Once);

        _mockRepoWrapper.Verify(r => r.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_DbExceptionThrownOnFirstSave_ShouldReturnFailure()
    {
        // Arrange
        SetupRepositoryWrapper(_existingSection);
        _mockRepoWrapper.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());

        var command = new DeletePartnersSectionCommand(_existingSection.Id);
        var handler = new DeletePartnersSectionHandler(_mockRepoWrapper.Object, _mockReorderService.Object, _mockBlobService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(PartnerSection)), result.Errors[0].Message);

        // Verify that no operations after the failed save were called
        _mockReorderService.Verify(s => s.RenumberPriorityAsync<PartnerSection>(null), Times.Never);
        _mockBlobService.Verify(b => b.DeleteFileInStorage(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReorderServiceFails_ShouldPropagateExceptionAndRollback()
    {
        // Arrange
        SetupRepositoryWrapper(_existingSection);
        _mockReorderService.Setup(s => s.RenumberPriorityAsync<PartnerSection>(null))
            .ThrowsAsync(new Exception("Reorder failed"));

        var command = new DeletePartnersSectionCommand(_existingSection.Id);
        var handler = new DeletePartnersSectionHandler(_mockRepoWrapper.Object, _mockReorderService.Object, _mockBlobService.Object);

        // Act & Assert
        // The transaction will not be completed, and the exception will propagate
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));

        // Verify that blob files were NOT deleted because the transaction was rolled back
        _mockBlobService.Verify(b => b.DeleteFileInStorage(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        // Verify SaveChanges was called once (for deletions), but the second call (after reordering) was not
        _mockRepoWrapper.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    private void SetupRepositoryWrapper(PartnerSection? entityToDelete = null, int firstSaveResult = 1, int secondSaveResult = 1)
    {
        _mockRepoWrapper.Setup(r => r.PartnerSectionsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PartnerSection>>()))
            .ReturnsAsync(entityToDelete);

        _mockRepoWrapper.SetupSequence(r => r.SaveChangesAsync())
            .ReturnsAsync(firstSaveResult)
            .ReturnsAsync(secondSaveResult);

        _mockRepoWrapper.Setup(r => r.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        // Setup Delete/DeleteRange to do nothing, just allow calls
        _mockRepoWrapper.Setup(r => r.ImageRepository.DeleteRange(It.IsAny<IEnumerable<Image>>()));
        _mockRepoWrapper.Setup(r => r.PartnerRepository.DeleteRange(It.IsAny<IEnumerable<Partner>>()));
        _mockRepoWrapper.Setup(r => r.PartnerSectionsRepository.Delete(It.IsAny<PartnerSection>()));
    }

    private void SetupReorderService()
    {
        _mockReorderService.Setup(s => s.RenumberPriorityAsync<PartnerSection>(null))
            .Returns(Task.CompletedTask);
    }
}
