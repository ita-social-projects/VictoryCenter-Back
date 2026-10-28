namespace VictoryCenter.BLL.Services.ReportExpendituresCategorySwapService;

public static class ReportExpendituresCategorySwapService
{
    public static async Task ProcessUpdatesAsync<TEntity, TDto>(
        List<TDto> recordsToUpdate,
        List<long> recordIdsToDelete,
        Dictionary<long, TEntity> existingEntities,
        ReportExpendituresCategorySwapContext<TEntity, TDto> context)
        where TEntity : class
        where TDto : class
    {
        if (recordsToUpdate.Count == 0)
        {
            return;
        }

        var occupiedCategories = GetOccupiedCategories(existingEntities, recordIdsToDelete, context);

        var (safeRecords, conflictingRecords) = ClassifyRecords(
            existingEntities, occupiedCategories, recordsToUpdate, context);

        await ProcessSafeRecordsAsync(safeRecords, existingEntities, occupiedCategories, context);

        await ProcessConflictingRecordsAsync(conflictingRecords, existingEntities, occupiedCategories, context);
    }

    private static Dictionary<long, long> GetOccupiedCategories<TEntity, TDto>(
        Dictionary<long, TEntity> existingEntities,
        List<long> recordIdsToDelete,
        ReportExpendituresCategorySwapContext<TEntity, TDto> context)
        where TEntity : class
        where TDto : class
    {
        return existingEntities.Values
            .Where(r => !recordIdsToDelete.Contains(context.GetEntityId(r)))
            .ToDictionary(context.GetEntityCategoryId, context.GetEntityId);
    }

    private static (List<TDto> safeRecords, List<TDto> conflictingRecords) ClassifyRecords<TEntity, TDto>(
        Dictionary<long, TEntity> existingEntities,
        Dictionary<long, long> occupiedCategories,
        List<TDto> recordsToUpdate,
        ReportExpendituresCategorySwapContext<TEntity, TDto> context)
        where TEntity : class
        where TDto : class
    {
        var safeRecords = new List<TDto>();
        var conflictingRecords = new List<TDto>();

        foreach (var record in recordsToUpdate)
        {
            long id = context.GetDtoId(record);
            long targetCategoryId = context.GetTargetCategoryId(record);

            bool isCategoryUnchanged = targetCategoryId == context.GetEntityCategoryId(existingEntities[id]);
            bool isTargetCategoryFree = !occupiedCategories.TryGetValue(targetCategoryId, out long occupantId) || occupantId == id;

            if (isCategoryUnchanged || isTargetCategoryFree)
            {
                safeRecords.Add(record);
            }
            else
            {
                conflictingRecords.Add(record);
            }
        }

        return (safeRecords, conflictingRecords);
    }

    private static async Task ProcessSafeRecordsAsync<TEntity, TDto>(
        List<TDto> safeRecords,
        Dictionary<long, TEntity> existingEntities,
        Dictionary<long, long> occupiedCategories,
        ReportExpendituresCategorySwapContext<TEntity, TDto> context)
        where TEntity : class
        where TDto : class
    {
        if (safeRecords.Count == 0)
        {
            return;
        }

        var entitiesToUpdate = new List<TEntity>();

        foreach (var record in safeRecords)
        {
            var entity = existingEntities[context.GetDtoId(record)];
            var oldCategoryId = context.GetEntityCategoryId(entity);

            context.ApplyEntityUpdates(entity, record);

            var newCategoryId = context.GetEntityCategoryId(entity);

            if (oldCategoryId != newCategoryId)
            {
                occupiedCategories.Remove(oldCategoryId);
                occupiedCategories[newCategoryId] = context.GetEntityId(entity);
            }

            entitiesToUpdate.Add(entity);
        }

        context.UpdateRangeInDb(entitiesToUpdate);
        await context.SaveChangesAsync();
    }

    private static async Task ProcessConflictingRecordsAsync<TEntity, TDto>(
        List<TDto> conflictingRecords,
        Dictionary<long, TEntity> existingEntities,
        Dictionary<long, long> occupiedCategories,
        ReportExpendituresCategorySwapContext<TEntity, TDto> context)
        where TEntity : class
        where TDto : class
    {
        if (conflictingRecords.Count == 0)
        {
            return;
        }

        long tempCategoryId = -1;
        TEntity? tempRecord = null;
        long targetForTempRecord = -1;

        try
        {
            while (conflictingRecords.Count > 0 || tempRecord != null)
            {
                if (tempRecord != null && await TryResolveTempRecordAsync(tempRecord, targetForTempRecord, occupiedCategories, context))
                {
                    tempRecord = null;
                    targetForTempRecord = -1;
                    continue;
                }

                if (await TryResolveReadyRecordAsync(conflictingRecords, existingEntities, occupiedCategories, context))
                {
                    continue;
                }

                if (tempRecord == null)
                {
                    if (tempCategoryId == -1)
                    {
                        tempCategoryId = await context.CreateTempCategoryAsync();
                    }

                    (tempRecord, targetForTempRecord) =
                        await ParkFirstConflictAsync(
                            conflictingRecords,
                            tempCategoryId,
                            existingEntities,
                            occupiedCategories,
                            context);
                }
            }
        }
        finally
        {
            await TryCleanupTempCategoryAsync(tempCategoryId, context);
        }
    }

    private static async Task<bool> TryResolveTempRecordAsync<TEntity, TDto>(
        TEntity tempRecord,
        long targetForTempRecord,
        Dictionary<long, long> occupiedCategories,
        ReportExpendituresCategorySwapContext<TEntity, TDto> context)
        where TEntity : class
        where TDto : class
    {
        bool isTargetFree = !occupiedCategories.TryGetValue(targetForTempRecord, out long occupantId) || occupantId == context.GetEntityId(tempRecord);

        if (!isTargetFree)
        {
            return false;
        }

        context.SetEntityCategoryId(tempRecord, targetForTempRecord);
        context.UpdateInDb(tempRecord);
        await context.SaveChangesAsync();

        occupiedCategories[targetForTempRecord] = context.GetEntityId(tempRecord);
        return true;
    }

    private static async Task<bool> TryResolveReadyRecordAsync<TEntity, TDto>(
        List<TDto> conflictingRecords,
        Dictionary<long, TEntity> existingEntities,
        Dictionary<long, long> occupiedCategories,
        ReportExpendituresCategorySwapContext<TEntity, TDto> context)
        where TEntity : class
        where TDto : class
    {
        var readyDto = conflictingRecords.Find(r =>
            !occupiedCategories.TryGetValue(context.GetTargetCategoryId(r), out long occupantId)
            || occupantId == context.GetDtoId(r));

        if (readyDto == null)
        {
            return false;
        }

        var entity = existingEntities[context.GetDtoId(readyDto)];
        var oldCategory = context.GetEntityCategoryId(entity);

        context.ApplyEntityUpdates(entity, readyDto);
        context.UpdateInDb(entity);
        await context.SaveChangesAsync();

        occupiedCategories.Remove(oldCategory);
        occupiedCategories[context.GetTargetCategoryId(readyDto)] = context.GetEntityId(entity);
        conflictingRecords.Remove(readyDto);

        return true;
    }

    private static async Task<(TEntity TempRecord, long TargetCategoryId)> ParkFirstConflictAsync<TEntity, TDto>(
        List<TDto> conflictingRecords,
        long tempCategoryId,
        Dictionary<long, TEntity> existingEntities,
        Dictionary<long, long> occupiedCategories,
        ReportExpendituresCategorySwapContext<TEntity, TDto> context)
        where TEntity : class
        where TDto : class
    {
        var conflictDto = conflictingRecords[0];
        var entity = existingEntities[context.GetDtoId(conflictDto)];
        var oldCategory = context.GetEntityCategoryId(entity);

        var tempRecord = entity;
        long targetForTempRecord = context.GetTargetCategoryId(conflictDto);

        context.ApplyEntityUpdates(entity, conflictDto);
        context.SetEntityCategoryId(entity, tempCategoryId);

        context.UpdateInDb(entity);
        await context.SaveChangesAsync();

        occupiedCategories.Remove(oldCategory);
        conflictingRecords.Remove(conflictDto);

        return (tempRecord, targetForTempRecord);
    }

    private static async Task TryCleanupTempCategoryAsync<TEntity, TDto>(
        long tempCategoryId,
        ReportExpendituresCategorySwapContext<TEntity, TDto> context)
        where TEntity : class
        where TDto : class
    {
        if (tempCategoryId == -1)
        {
            return;
        }

        try
        {
            await context.CleanupTempCategoryAsync(tempCategoryId);
        }
        catch
        {
            // Intentionally suppressed to avoid masking the primary exception.
            // Transaction rollback will handle cleanup.
        }
    }
}
