using System.Linq.Expressions;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Services.ReorderService;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.ServiceTests;

public class TestOrderableEntity : IOrderableEntity
{
    public long Id { get; set; }
    public long Priority { get; set; }
    public string? GroupId { get; set; }
}

public class ReorderServiceTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IRepositoryBase<TestOrderableEntity>> _repositoryMock;
    private readonly ReorderService _reorderService;

    public ReorderServiceTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _repositoryMock = new Mock<IRepositoryBase<TestOrderableEntity>>();
        _repositoryWrapperMock.Setup(x => x.GetRepository<TestOrderableEntity>())
            .Returns(_repositoryMock.Object);
        _reorderService = new ReorderService(_repositoryWrapperMock.Object);
    }

    [Fact]
    public async Task SwapElements_NullIdsOrder_DoesNothing()
    {
        // Arrange
        List<long>? idsOrder = null;
        Expression<Func<TestOrderableEntity, long>> idSelector = x => x.Id;

        // Act
        await _reorderService.SwapElementsAsync(idsOrder, idSelector);

        // Assert
        _repositoryMock.Verify(x => x.GetAllAsync(It.IsAny<QueryOptions<TestOrderableEntity>>()), Times.Never);
        _repositoryWrapperMock.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task SwapElements_EmptyIdsOrder_DoesNothing()
    {
        // Arrange
        var idsOrder = new List<long>();
        Expression<Func<TestOrderableEntity, long>> idSelector = x => x.Id;

        // Act
        await _reorderService.SwapElementsAsync(idsOrder, idSelector);

        // Assert
        _repositoryMock.Verify(x => x.GetAllAsync(It.IsAny<QueryOptions<TestOrderableEntity>>()), Times.Never);
        _repositoryWrapperMock.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task SwapElements_ExceedsMaxElementsCount_ThrowsReorderException()
    {
        // Arrange
        var idsOrder = Enumerable.Range(1, ReorderConstants.MaxElementsSwapCount + 1).Select(x => (long)x).ToList();
        Expression<Func<TestOrderableEntity, long>> idSelector = x => x.Id;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ReorderException>(() =>
            _reorderService.SwapElementsAsync(idsOrder, idSelector));

        Assert.Equal(ReorderConstants.ExceededMaxElementsSwapCount(idsOrder.Count), exception.Message);
        _repositoryMock.Verify(x => x.GetAllAsync(It.IsAny<QueryOptions<TestOrderableEntity>>()), Times.Never);
    }

    [Fact]
    public async Task SwapElements_DuplicateIdsInOrder_RemovesDuplicates()
    {
        // Arrange
        var idsOrder = new List<long> { 1, 2, 2, 3 }; // Contains duplicate
        var expectedIds = new List<long> { 1, 2, 3 };
        var entities = new List<TestOrderableEntity>
        {
            new() { Id = 1, Priority = 1 },
            new() { Id = 2, Priority = 2 },
            new() { Id = 3, Priority = 3 }
        };
        Expression<Func<TestOrderableEntity, long>> idSelector = x => x.Id;

        _repositoryMock.Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<TestOrderableEntity>>()))
            .ReturnsAsync(entities);

        // Act
        await _reorderService.SwapElementsAsync(idsOrder, idSelector);

        // Assert
        _repositoryMock.Verify(x => x.Update(It.IsAny<TestOrderableEntity>()), Times.Exactly(6)); // 3 temp + 3 final
        _repositoryWrapperMock.Verify(x => x.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task SwapElements_NotAllEntitiesFound_ThrowsReorderException()
    {
        // Arrange
        var idsOrder = new List<long> { 1, 2, 3 };
        var entities = new List<TestOrderableEntity>
        {
            new() { Id = 1, Priority = 1 },
            new() { Id = 2, Priority = 2 }
        };
        Expression<Func<TestOrderableEntity, long>> idSelector = x => x.Id;

        _repositoryMock.Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<TestOrderableEntity>>()))
            .ReturnsAsync(entities);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ReorderException>(() =>
            _reorderService.SwapElementsAsync(idsOrder, idSelector));

        Assert.Equal(ReorderConstants.NotAllEntitiesFoundForReorder(foundCount: 2, expectedCount: 3), exception.Message);
    }

    [Fact]
    public async Task SwapElements_ValidRequest_SwapsElementsSuccessfully()
    {
        // Arrange
        var idsOrder = new List<long> { 2, 1, 3 }; // Reorder: second becomes first, first becomes second, third stays
        var entities = new List<TestOrderableEntity>
        {
            new() { Id = 1, Priority = 1 },
            new() { Id = 2, Priority = 2 },
            new() { Id = 3, Priority = 3 }
        };
        Expression<Func<TestOrderableEntity, long>> idSelector = x => x.Id;

        _repositoryMock.Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<TestOrderableEntity>>()))
            .ReturnsAsync(entities);

        // Act
        await _reorderService.SwapElementsAsync(idsOrder, idSelector);

        // Assert
        _repositoryMock.Verify(x => x.Update(It.IsAny<TestOrderableEntity>()), Times.Exactly(6));

        _repositoryWrapperMock.Verify(x => x.SaveChangesAsync(), Times.Exactly(2));

        Assert.Equal(1, entities.First(e => e.Id == 2).Priority);
        Assert.Equal(2, entities.First(e => e.Id == 1).Priority);
        Assert.Equal(3, entities.First(e => e.Id == 3).Priority);
    }

    [Fact]
    public async Task SwapElements_WithGroupSelector_SwapsOnlyElementsInSpecifiedGroup()
    {
        // Arrange
        var idsOrder = new List<long> { 2, 1 }; // Only reorder elements from GroupA
        var entities = new List<TestOrderableEntity>
        {
            new() { Id = 1, Priority = 1, GroupId = "GroupA" },
            new() { Id = 2, Priority = 2, GroupId = "GroupA" },
            new() { Id = 3, Priority = 3, GroupId = "GroupB" } // Different group - should not be affected
        };
        Expression<Func<TestOrderableEntity, long>> idSelector = x => x.Id;
        Expression<Func<TestOrderableEntity, bool>> groupSelector = x => x.GroupId == "GroupA";

        _repositoryMock.Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<TestOrderableEntity>>()))
            .ReturnsAsync(entities);

        // Act
        await _reorderService.SwapElementsAsync(idsOrder, idSelector, groupSelector);

        // Assert
        _repositoryMock.Verify(x => x.Update(It.IsAny<TestOrderableEntity>()), Times.Exactly(4));

        _repositoryWrapperMock.Verify(x => x.SaveChangesAsync(), Times.Exactly(2));

        Assert.Equal(1, entities.First(e => e.Id == 2).Priority);
        Assert.Equal(2, entities.First(e => e.Id == 1).Priority);
        Assert.Equal(3, entities.First(e => e.Id == 3).Priority);
    }

    [Fact]
    public async Task GetNextDisplayOrder_NoExistingEntities_ReturnsOne()
    {
        // Arrange
        _repositoryMock.Setup(x => x.MaxAsync(It.IsAny<Expression<Func<TestOrderableEntity, long>>>(), null))
            .ReturnsAsync((long?)null);

        // Act
        var result = await _reorderService.GetNextDisplayOrderAsync<TestOrderableEntity>();

        // Assert
        Assert.Equal(1, result);
        _repositoryMock.Verify(x => x.MaxAsync(It.IsAny<Expression<Func<TestOrderableEntity, long>>>(), null), Times.Once);
    }

    [Fact]
    public async Task GetNextDisplayOrder_ExistingEntities_ReturnsMaxPlusOne()
    {
        // Arrange
        const long maxPriority = 5;
        _repositoryMock.Setup(x => x.MaxAsync(It.IsAny<Expression<Func<TestOrderableEntity, long>>>(), null))
            .ReturnsAsync(maxPriority);

        // Act
        var result = await _reorderService.GetNextDisplayOrderAsync<TestOrderableEntity>();

        // Assert
        Assert.Equal(maxPriority + 1, result);
        _repositoryMock.Verify(x => x.MaxAsync(It.IsAny<Expression<Func<TestOrderableEntity, long>>>(), null), Times.Once);
    }

    [Fact]
    public async Task GetNextDisplayOrder_WithGroupSelector_FiltersEntitiesByGroup()
    {
        // Arrange
        const long maxPriority = 3;
        Expression<Func<TestOrderableEntity, bool>> groupSelector = x => x.GroupId == "GroupA";
        _repositoryMock.Setup(x => x.MaxAsync(It.IsAny<Expression<Func<TestOrderableEntity, long>>>(), groupSelector))
            .ReturnsAsync(maxPriority);

        // Act
        var result = await _reorderService.GetNextDisplayOrderAsync(groupSelector);

        // Assert
        Assert.Equal(maxPriority + 1, result);
        _repositoryMock.Verify(x => x.MaxAsync(It.IsAny<Expression<Func<TestOrderableEntity, long>>>(), groupSelector), Times.Once);
    }

    [Fact]
    public async Task RenumberPriorityAsync_NoEntities_DoesNothing()
    {
        // Arrange
        var entities = new List<TestOrderableEntity>();
        _repositoryMock.Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<TestOrderableEntity>>()))
            .ReturnsAsync(entities);

        // Act
        await _reorderService.RenumberPriorityAsync<TestOrderableEntity>();

        // Assert
        _repositoryMock.Verify(x => x.Update(It.IsAny<TestOrderableEntity>()), Times.Never);
    }

    [Fact]
    public async Task RenumberPriorityAsync_EntitiesAlreadySequential_DoesNotUpdate()
    {
        // Arrange
        var entities = new List<TestOrderableEntity>
    {
        new() { Id = 1, Priority = 1 },
        new() { Id = 2, Priority = 2 },
        new() { Id = 3, Priority = 3 }
    };
        _repositoryMock.Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<TestOrderableEntity>>()))
            .ReturnsAsync(entities);

        // Act
        await _reorderService.RenumberPriorityAsync<TestOrderableEntity>();

        // Assert
        _repositoryMock.Verify(x => x.Update(It.IsAny<TestOrderableEntity>()), Times.Never);
        _repositoryMock.Verify(x => x.GetAllAsync(It.IsAny<QueryOptions<TestOrderableEntity>>()), Times.Once);
    }

    [Fact]
    public async Task RenumberPriorityAsync_EntitiesWithGaps_RenumbersSequentially()
    {
        // Arrange
        var entities = new List<TestOrderableEntity>
        {
            new() { Id = 1, Priority = 5 },
            new() { Id = 2, Priority = 10 },
            new() { Id = 3, Priority = 15 }
        };
        _repositoryMock.Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<TestOrderableEntity>>()))
            .ReturnsAsync(entities);

        // Act
        await _reorderService.RenumberPriorityAsync<TestOrderableEntity>();

        // Assert
        _repositoryMock.Verify(x => x.Update(It.Is<TestOrderableEntity>(e => e.Id == 1 && e.Priority == 1)), Times.Once);
        _repositoryMock.Verify(x => x.Update(It.Is<TestOrderableEntity>(e => e.Id == 2 && e.Priority == 2)), Times.Once);
        _repositoryMock.Verify(x => x.Update(It.Is<TestOrderableEntity>(e => e.Id == 3 && e.Priority == 3)), Times.Once);
    }

    [Fact]
    public async Task RenumberPriorityAsync_MixedSequentialAndGaps_OnlyUpdatesNecessaryEntities()
    {
        // Arrange
        var entities = new List<TestOrderableEntity>
        {
            new() { Id = 1, Priority = 1 }, // Already correct, should not be updated
            new() { Id = 2, Priority = 5 }, // Should be updated to 2
            new() { Id = 3, Priority = 3 }, // Already correct, should not be updated
            new() { Id = 4, Priority = 10 } // Should be updated to 4
        };
        _repositoryMock.Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<TestOrderableEntity>>()))
            .ReturnsAsync(entities);

        // Act
        await _reorderService.RenumberPriorityAsync<TestOrderableEntity>();

        // Assert
        _repositoryMock.Verify(x => x.Update(It.Is<TestOrderableEntity>(e => e.Id == 1)), Times.Never);
        _repositoryMock.Verify(x => x.Update(It.Is<TestOrderableEntity>(e => e.Id == 2 && e.Priority == 2)), Times.Once);
        _repositoryMock.Verify(x => x.Update(It.Is<TestOrderableEntity>(e => e.Id == 3)), Times.Never);
        _repositoryMock.Verify(x => x.Update(It.Is<TestOrderableEntity>(e => e.Id == 4 && e.Priority == 4)), Times.Once);
    }

    [Fact]
    public async Task RenumberPriorityAsync_WithGroupSelector_FiltersEntitiesByGroup()
    {
        // Arrange
        var entities = new List<TestOrderableEntity>
    {
        new() { Id = 1, Priority = 5, GroupId = "GroupA" },
        new() { Id = 2, Priority = 10, GroupId = "GroupA" }
    };
        Expression<Func<TestOrderableEntity, bool>> groupSelector = x => x.GroupId == "GroupA";
        _repositoryMock.Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<TestOrderableEntity>>()))
            .ReturnsAsync(entities);

        // Act
        await _reorderService.RenumberPriorityAsync(groupSelector);

        // Assert
        _repositoryMock.Verify(x => x.GetAllAsync(It.IsAny<QueryOptions<TestOrderableEntity>>()), Times.Once);
        _repositoryMock.Verify(x => x.Update(It.Is<TestOrderableEntity>(e => e.Id == 1 && e.Priority == 1)), Times.Once);
        _repositoryMock.Verify(x => x.Update(It.Is<TestOrderableEntity>(e => e.Id == 2 && e.Priority == 2)), Times.Once);
    }
}
