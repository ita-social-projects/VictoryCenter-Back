namespace VictoryCenter.BLL.Services.ReportExpendituresCategorySwapService;

public class ReportExpendituresCategorySwapContext<TEntity, TDto>
    where TEntity : class
    where TDto : class
{
    public required Func<TEntity, long> GetEntityId { get; init; }
    public required Func<TEntity, long> GetEntityCategoryId { get; init; }
    public required Action<TEntity, long> SetEntityCategoryId { get; init; }

    public required Func<TDto, long> GetDtoId { get; init; }
    public required Func<TDto, long> GetTargetCategoryId { get; init; }

    public required Action<TEntity, TDto> ApplyEntityUpdates { get; init; }
    public required Action<IEnumerable<TEntity>> UpdateRangeInDb { get; init; }
    public required Action<TEntity> UpdateInDb { get; init; }
    public required Func<Task> SaveChangesAsync { get; init; }
    public required Func<Task<long>> CreateTempCategoryAsync { get; init; }
    public required Func<Task> CleanupTempCategoryAsync { get; init; }
}
