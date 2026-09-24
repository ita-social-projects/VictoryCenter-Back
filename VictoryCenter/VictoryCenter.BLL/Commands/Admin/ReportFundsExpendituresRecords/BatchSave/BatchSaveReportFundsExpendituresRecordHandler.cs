using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.Interfaces.ReportFundsExpendituresRecordHelper;
using VictoryCenter.BLL.Notifications.ReportFunds;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.BatchSave;

public class BatchSaveReportFundsExpendituresRecordHandler
    : IRequestHandler<BatchSaveReportFundsExpendituresRecordCommand, Result<Unit>>
{
    private readonly IMediator _mediator;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<BatchSaveReportFundsExpendituresRecordCommand> _validator;
    private readonly IMapper _mapper;
    private readonly IReportFundsExpendituresRecordHelper _helper;

    public BatchSaveReportFundsExpendituresRecordHandler(
        IMediator mediator,
        IRepositoryWrapper repositoryWrapper,
        IValidator<BatchSaveReportFundsExpendituresRecordCommand> validator,
        IMapper mapper,
        IReportFundsExpendituresRecordHelper helper)
    {
        _mediator = mediator;
        _validator = validator;
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _helper = helper;
    }

    public async Task<Result<Unit>> Handle(
        BatchSaveReportFundsExpendituresRecordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var settingsResult = await _helper.GetAndValidateSettingsAsync();
            if (settingsResult.IsFailed)
            {
                return Result.Fail<Unit>(settingsResult.Errors);
            }

            var dto = request.BatchSaveReportFundsExpendituresRecordsDto;

            var targetIds = dto.RecordsToUpdate
                .Select(r => r.Id)
                .Concat(dto.RecordIdsToDelete)
                .ToList();

            var existingRecordsDict = new Dictionary<long, ReportFundsExpendituresRecord>();

            if (targetIds.Count > 0)
            {
                existingRecordsDict = (await _repositoryWrapper.ReportFundsExpendituresRecordsRepository
                    .GetAllAsync(new QueryOptions<ReportFundsExpendituresRecord>
                    {
                        Filter = record => targetIds.Contains(record.Id)
                    }))
                    .ToDictionary(r => r.Id);
            }

            var existingRecordsValidationResult = ValidateExistingRecords(targetIds, existingRecordsDict);
            if (existingRecordsValidationResult.IsFailed)
            {
                return existingRecordsValidationResult;
            }

            var recordsToValidate = dto.RecordsToCreate
                .Select(c => (CategoryId: c.CategoryId, Type: c.Type))
                .Concat(
                    dto.RecordsToUpdate
                        .Where(u => u.CategoryId != existingRecordsDict[u.Id].CategoryId)
                        .Select(u => (CategoryId: u.CategoryId, Type: existingRecordsDict[u.Id].Type)))
                .ToList();

            if (recordsToValidate.Count > 0)
            {
                var categoryIdsToValidate = recordsToValidate
                    .Select(i => i.CategoryId)
                    .ToList();

                var categoriesDict = (await _repositoryWrapper.ReportFundsExpendituresCategoriesRepository
                    .GetAllAsync(new QueryOptions<ReportFundsExpendituresCategory>
                    {
                        Filter = entity => categoryIdsToValidate.Contains(entity.Id)
                    }))
                    .ToDictionary(c => c.Id);

                var touchedRecordIds = dto.RecordIdsToDelete
                    .Concat(dto.RecordsToUpdate.Select(u => u.Id))
                    .ToList();

                var duplicateRecordsInCategory = await _repositoryWrapper.ReportFundsExpendituresRecordsRepository
                    .GetAllAsync(new QueryOptions<ReportFundsExpendituresRecord>
                    {
                        Filter = entity => categoryIdsToValidate.Contains(entity.CategoryId) &&
                                           !touchedRecordIds.Contains(entity.Id)
                    });

                var categoriesValidationResult = ValidateCategories(
                    recordsToValidate,
                    categoriesDict,
                    duplicateRecordsInCategory);

                if (categoriesValidationResult.IsFailed)
                {
                    return categoriesValidationResult;
                }
            }

            await using var scope = await _repositoryWrapper.BeginTransactionAsync();

            decimal exchangeRate = settingsResult.Value.ExchangeRate;

            HandleDeletions(dto, existingRecordsDict);
            HandleUpdates(dto, existingRecordsDict, exchangeRate);
            await HandleCreations(dto, exchangeRate);

            int affectedRows = await _repositoryWrapper.SaveChangesAsync();
            await scope.CommitAsync(cancellationToken);

            if (affectedRows > 0)
            {
                await _mediator.Publish(new ReportFundsChangedNotification(), CancellationToken.None);
            }

            return Result.Ok(Unit.Value);
        }
        catch (ValidationException validationException)
        {
            return Result.Fail<Unit>(
                validationException.Errors.Select(error => error.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<Unit>(
                ErrorMessagesConstants.FailedToSaveEntitiesInDatabase(typeof(ReportFundsExpendituresRecord)));
        }
    }

    private static Result<Unit> ValidateExistingRecords(
        IReadOnlyCollection<long> targetIds, IReadOnlyDictionary<long, ReportFundsExpendituresRecord> existingRecordsDict)
    {
        var nonExistingRecordIds = targetIds
            .Where(id => !existingRecordsDict.ContainsKey(id))
            .ToList();

        if (nonExistingRecordIds.Count > 0)
        {
            return Result.Fail<Unit>(ErrorMessagesConstants.NotFound(
                nonExistingRecordIds,
                typeof(ReportFundsExpendituresRecord)));
        }

        return Result.Ok(Unit.Value);
    }

    private static Result<Unit> ValidateCategories(
        IReadOnlyCollection<(long CategoryId, ReportFundsExpendituresType Type)> recordsToValidate,
        IReadOnlyDictionary<long, ReportFundsExpendituresCategory> categoriesDict,
        IEnumerable<ReportFundsExpendituresRecord> duplicateRecordsInCategory)
    {
        var nonExistingCategoryIds = recordsToValidate
            .Select(r => r.CategoryId)
            .Where(id => !categoriesDict.ContainsKey(id))
            .ToList();

        if (nonExistingCategoryIds.Count > 0)
        {
            return Result.Fail<Unit>(ErrorMessagesConstants.NotFound(
                nonExistingCategoryIds,
                typeof(ReportFundsExpendituresCategory)));
        }

        foreach(var record in recordsToValidate)
        {
            var category = categoriesDict[record.CategoryId];

            if (category.Type != record.Type)
            {
                return Result.Fail<Unit>(
                    ReportFundsExpendituresRecordConstants.CategoryTypeMustMatchRecordType);
            }
        }

        if (duplicateRecordsInCategory.Any())
        {
            return Result.Fail<Unit>(
                    ReportFundsExpendituresRecordConstants.CategoryAlreadyHasRecord);
        }

        return Result.Ok(Unit.Value);
    }

    private void HandleDeletions(
        BatchSaveReportFundsExpendituresRecordsDto dto,
        IDictionary<long, ReportFundsExpendituresRecord> existingRecordsDict)
    {
        if (dto.RecordIdsToDelete.Count == 0)
        {
            return;
        }

        var recordsToDelete = dto.RecordIdsToDelete
            .Select(id => existingRecordsDict[id])
            .ToList();

        _repositoryWrapper.ReportFundsExpendituresRecordsRepository.DeleteRange(recordsToDelete);
    }

    private void HandleUpdates(
        BatchSaveReportFundsExpendituresRecordsDto dto,
        IDictionary<long, ReportFundsExpendituresRecord> existingRecordsDict,
        decimal exchangeRate)
    {
        if (dto.RecordsToUpdate.Count == 0)
        {
            return;
        }

        var entitiesToUpdate = new List<ReportFundsExpendituresRecord>();

        foreach (var recordToUpdateDto in dto.RecordsToUpdate)
        {
            var existingEntity = existingRecordsDict[recordToUpdateDto.Id];
            _mapper.Map(recordToUpdateDto, existingEntity);

            (existingEntity.AmountUah, existingEntity.AmountUsd) = _helper.CalculateAmounts(
                recordToUpdateDto.Amount!.Value,
                recordToUpdateDto.Currency,
                exchangeRate);

            entitiesToUpdate.Add(existingEntity);
        }

        _repositoryWrapper.ReportFundsExpendituresRecordsRepository.UpdateRange(entitiesToUpdate);
    }

    private async Task HandleCreations(
        BatchSaveReportFundsExpendituresRecordsDto dto,
        decimal exchangeRate)
    {
        if (dto.RecordsToCreate.Count == 0)
        {
            return;
        }

        var newEntities = new List<ReportFundsExpendituresRecord>();

        foreach(var recordToCreateDto in dto.RecordsToCreate)
        {
            var entity = _mapper.Map<ReportFundsExpendituresRecord>(recordToCreateDto);
            (entity.AmountUah, entity.AmountUsd) = _helper.CalculateAmounts(
                recordToCreateDto.Amount!.Value,
                recordToCreateDto.Currency,
                exchangeRate);

            newEntities.Add(entity);
        }

        await _repositoryWrapper.ReportFundsExpendituresRecordsRepository.CreateRangeAsync(newEntities);
    }
}
