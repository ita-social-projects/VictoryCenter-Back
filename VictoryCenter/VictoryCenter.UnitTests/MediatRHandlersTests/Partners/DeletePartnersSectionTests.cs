using System.Linq.Expressions;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Partners.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Partners;

public class DeletePartnersSectionTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepoWrapper;
    private readonly Mock<IReorderService> _mockReorderService;

    // Sample data for an existing section
    private readonly PartnerSection _existingSectionEntity = new()
    {
        Id = 1,
        Title = "Existing Section",
        Priority = 1,
        Partners =
        [
            new() { Id = 10, Description = "Partner A" }
        ]
    };

    public DeletePartnersSectionTests()
    {
        _mockRepoWrapper = new Mock<IRepositoryWrapper>();
        _mockReorderService = new Mock<IReorderService>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1000)]
    public async Task Handle_EntityNotFound_ShouldReturnFailure(long sectionId)
    {
        // Arrange
        SetupRepositoryWrapper(null);
        var command = new DeletePartnersSectionCommand(sectionId);
        var handler = new DeletePartnersSectionHandler(_mockRepoWrapper.Object, _mockReorderService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.NotFound(sectionId, typeof(PartnerSection)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_EntityExists_ShouldReturnOk()
    {
        // Arrange
        SetupRepositoryWrapper(_existingSectionEntity);
        SetupReorderService();
        var command = new DeletePartnersSectionCommand(_existingSectionEntity.Id);
        var handler = new DeletePartnersSectionHandler(_mockRepoWrapper.Object, _mockReorderService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(_existingSectionEntity.Id, result.Value);

        // Verify that the correct methods were called
        _mockRepoWrapper.Verify(r => r.PartnerSectionsRepository.Delete(_existingSectionEntity), Times.Once);
        _mockRepoWrapper.Verify(r => r.SaveChangesAsync(), Times.Exactly(1));
        _mockReorderService.Verify(s => s.RenumberPriorityAsync<PartnerSection>(It.IsAny<Expression<Func<PartnerSection, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DbUpdateExceptionThrown_ShouldReturnFailure()
    {
        // Arrange
        SetupRepositoryWrapper(_existingSectionEntity);
        SetupReorderService();

        _mockRepoWrapper.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());

        var command = new DeletePartnersSectionCommand(_existingSectionEntity.Id);
        var handler = new DeletePartnersSectionHandler(_mockRepoWrapper.Object, _mockReorderService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(PartnerSection)), result.Errors[0].Message);
    }

    private void SetupReorderService()
    {
        _mockReorderService
            .Setup(service => service.RenumberPriorityAsync<PartnerSection>(
                It.IsAny<Expression<Func<PartnerSection, bool>>>()))
            .Returns(Task.CompletedTask);
    }

    private void SetupRepositoryWrapper(PartnerSection? entityToDelete)
    {
        _mockRepoWrapper.Setup(
            repoWrapper => repoWrapper.PartnerSectionsRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<PartnerSection>>()))
            .ReturnsAsync(entityToDelete);

        _mockRepoWrapper.Setup(repoWrapper => repoWrapper.SaveChangesAsync())
            .ReturnsAsync(1);

        _mockRepoWrapper.Setup(repoWrapper => repoWrapper.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        _mockRepoWrapper.Setup(repoWrapper =>
            repoWrapper.PartnerSectionsRepository.Delete(It.IsAny<PartnerSection>()));
    }
}
