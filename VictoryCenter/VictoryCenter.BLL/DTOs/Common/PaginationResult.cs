namespace VictoryCenter.BLL.DTOs.Common;

public record PaginationResult<T>(T[] Items, long TotalItemsCount)
    where T : class;
