using VictoryCenter.BLL.Services.ReportExpendituresCategorySwapService;

namespace VictoryCenter.UnitTests.ServiceTests;

public class ReportExpendituresCategorySwapServiceTests
{
    [Fact]
    public async Task ProcessUpdatesAsync_WhenNoRecordsToUpdate_ShouldDoNothing()
    {
        // Arrange
        var entities = CreateEntities((1, 1), (2, 2));
        var recordsToUpdate = new List<TestUpdateDto>();
        var tracker = CreateTracker();
        var context = CreateContext(tracker);

        // Act
        await ReportExpendituresCategorySwapService.ProcessUpdatesAsync(
            recordsToUpdate,
            recordIdsToDelete: [],
            entities,
            context);

        // Assert
        Assert.Equal(0, tracker.UpdateRangeCalls);
        Assert.Equal(0, tracker.UpdateCalls);
        Assert.Equal(0, tracker.SaveChangesCalls);
        Assert.Equal(0, tracker.CreateTempCategoryCalls);
        Assert.Equal(0, tracker.CleanupTempCategoryCalls);
        Assert.Equal(1, entities[1].CategoryId);
        Assert.Equal(2, entities[2].CategoryId);
    }

    [Fact]
    public async Task ProcessUpdatesAsync_WhenOnlySafeUpdates_ShouldUpdateWithoutTempCategory()
    {
        // Arrange
        var entities = CreateEntities((1, 1), (2, 2));
        var recordsToUpdate = CreateUpdates((1, 3), (2, 4));
        var tracker = CreateTracker();
        var context = CreateContext(tracker);

        // Act
        await ReportExpendituresCategorySwapService.ProcessUpdatesAsync(
            recordsToUpdate,
            recordIdsToDelete: [],
            entities,
            context);

        // Assert
        Assert.Equal(1, tracker.UpdateRangeCalls);
        Assert.Equal(0, tracker.UpdateCalls);
        Assert.Equal(1, tracker.SaveChangesCalls);
        Assert.Equal(0, tracker.CreateTempCategoryCalls);
        Assert.Equal(0, tracker.CleanupTempCategoryCalls);
        Assert.Equal(3, entities[1].CategoryId);
        Assert.Equal(4, entities[2].CategoryId);
    }

    [Fact]
    public async Task ProcessUpdatesAsync_WhenTransitiveChain_ShouldResolveWithoutTempCategory()
    {
        // Arrange
        var entities = CreateEntities((1, 1), (2, 2));
        var recordsToUpdate = CreateUpdates((1, 2), (2, 3));
        var tracker = CreateTracker();
        var context = CreateContext(tracker);

        // Act
        await ReportExpendituresCategorySwapService.ProcessUpdatesAsync(
            recordsToUpdate,
            recordIdsToDelete: [],
            entities,
            context);

        // Assert
        Assert.Equal(1, tracker.UpdateRangeCalls);
        Assert.Equal(1, tracker.UpdateCalls);
        Assert.Equal(2, tracker.SaveChangesCalls);
        Assert.Equal(0, tracker.CreateTempCategoryCalls);
        Assert.Equal(0, tracker.CleanupTempCategoryCalls);
        Assert.Equal(2, entities[1].CategoryId);
        Assert.Equal(3, entities[2].CategoryId);
    }

    [Fact]
    public async Task ProcessUpdatesAsync_WhenDirectSwap_ShouldCreateAndCleanupTempCategory()
    {
        // Arrange
        var entities = CreateEntities((1, 1), (2, 2));
        var recordsToUpdate = CreateUpdates((1, 2), (2, 1));
        var tracker = CreateTracker();
        var context = CreateContext(tracker);

        // Act
        await ReportExpendituresCategorySwapService.ProcessUpdatesAsync(
            recordsToUpdate,
            recordIdsToDelete: [],
            entities,
            context);

        // Assert
        Assert.Equal(0, tracker.UpdateRangeCalls);
        Assert.Equal(3, tracker.UpdateCalls);
        Assert.Equal(3, tracker.SaveChangesCalls);
        Assert.Equal(1, tracker.CreateTempCategoryCalls);
        Assert.Equal(1, tracker.CleanupTempCategoryCalls);
        Assert.Equal(tracker.CreatedTempCategoryId, tracker.CleanupTempCategoryId);
        Assert.Equal(2, entities[1].CategoryId);
        Assert.Equal(1, entities[2].CategoryId);
    }

    [Fact]
    public async Task ProcessUpdatesAsync_WhenExceptionThrownDuringSwap_ShouldStillCleanupTempCategory()
    {
        // Arrange
        var entities = CreateEntities((1, 1), (2, 2));
        var recordsToUpdate = CreateUpdates((1, 2), (2, 1));
        var tracker = CreateTracker(throwOnSaveAfterTempCreated: true);
        var context = CreateContext(tracker);

        // Act
        var action = async () => await ReportExpendituresCategorySwapService.ProcessUpdatesAsync(
            recordsToUpdate,
            recordIdsToDelete: [],
            entities,
            context);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(action);
        Assert.Equal(1, tracker.CreateTempCategoryCalls);
        Assert.Equal(1, tracker.CleanupTempCategoryCalls);
        Assert.Equal(tracker.CreatedTempCategoryId, tracker.CleanupTempCategoryId);
    }

    private static Dictionary<long, TestEntity> CreateEntities(params (long Id, long CategoryId)[] entries)
    {
        return entries.ToDictionary(
            x => x.Id,
            x => new TestEntity
            {
                Id = x.Id,
                CategoryId = x.CategoryId,
            });
    }

    private static List<TestUpdateDto> CreateUpdates(params (long Id, long TargetCategoryId)[] entries)
    {
        return entries.Select(x => new TestUpdateDto
        {
            Id = x.Id,
            TargetCategoryId = x.TargetCategoryId,
        }).ToList();
    }

    private static SwapExecutionTracker CreateTracker(bool throwOnSaveAfterTempCreated = false)
    {
        return new SwapExecutionTracker
        {
            ThrowOnSaveAfterTempCreated = throwOnSaveAfterTempCreated,
        };
    }

    private static ReportExpendituresCategorySwapContext<TestEntity, TestUpdateDto> CreateContext(SwapExecutionTracker tracker)
    {
        return new ReportExpendituresCategorySwapContext<TestEntity, TestUpdateDto>
        {
            GetEntityId = entity => entity.Id,
            GetEntityCategoryId = entity => entity.CategoryId,
            SetEntityCategoryId = (entity, categoryId) => entity.CategoryId = categoryId,
            GetDtoId = dto => dto.Id,
            GetTargetCategoryId = dto => dto.TargetCategoryId,
            ApplyEntityUpdates = (entity, dto) => entity.CategoryId = dto.TargetCategoryId,
            UpdateRangeInDb = _ => tracker.UpdateRangeCalls++,
            UpdateInDb = _ => tracker.UpdateCalls++,
            SaveChangesAsync = () =>
            {
                tracker.SaveChangesCalls++;
                if (tracker.ThrowOnSaveAfterTempCreated && tracker.CreateTempCategoryCalls > 0)
                {
                    throw new InvalidOperationException("Simulated deadlock");
                }

                return Task.CompletedTask;
            },
            CreateTempCategoryAsync = () =>
            {
                tracker.CreateTempCategoryCalls++;
                tracker.CreatedTempCategoryId = 999;
                return Task.FromResult(tracker.CreatedTempCategoryId.Value);
            },
            CleanupTempCategoryAsync = tempCategoryId =>
            {
                tracker.CleanupTempCategoryCalls++;
                tracker.CleanupTempCategoryId = tempCategoryId;
                return Task.CompletedTask;
            },
        };
    }

    private class TestEntity
    {
        public long Id { get; init; }

        public long CategoryId { get; set; }
    }

    private class TestUpdateDto
    {
        public long Id { get; init; }

        public long TargetCategoryId { get; init; }
    }

    private class SwapExecutionTracker
    {
        public int UpdateRangeCalls { get; set; }

        public int UpdateCalls { get; set; }

        public int SaveChangesCalls { get; set; }

        public int CreateTempCategoryCalls { get; set; }

        public int CleanupTempCategoryCalls { get; set; }

        public bool ThrowOnSaveAfterTempCreated { get; init; }

        public long? CreatedTempCategoryId { get; set; }

        public long? CleanupTempCategoryId { get; set; }
    }
}
