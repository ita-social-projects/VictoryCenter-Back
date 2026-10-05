using System.Data;
using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Notifications.ReportFunds;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.BatchSave;

public class BatchSaveReportProgramExpendituresRecordHandler
    : IRequestHandler<BatchSaveReportProgramExpendituresRecordCommand, Result<Unit>>
{
    private readonly IMediator _mediator;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<BatchSaveReportProgramExpendituresRecordCommand> _validator;
    private readonly IMapper _mapper;
    private readonly ILogger<BatchSaveReportProgramExpendituresRecordHandler> _logger;

    public BatchSaveReportProgramExpendituresRecordHandler(
        IMediator mediator,
        IRepositoryWrapper repositoryWrapper,
        IValidator<BatchSaveReportProgramExpendituresRecordCommand> validator,
        IMapper mapper,
        ILogger<BatchSaveReportProgramExpendituresRecordHandler> logger)
    {
        _mediator = mediator;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(BatchSaveReportProgramExpendituresRecordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Fail<Unit>(
                    validationResult.Errors.Select(error => error.ErrorMessage));
            }

            var dto = request.BatchSaveReportProgramExpendituresRecordsDto;

            var targetIds = dto.RecordsToUpdate
                .Select(r => r.Id)
                .Concat(dto.RecordIdsToDelete)
                .ToList();

            var existingRecordsDict = new Dictionary<long, ReportProgramExpendituresRecord>();

            await using var scope = await _repositoryWrapper.BeginTransactionAsync(cancellationToken);

            if (targetIds.Count > 0)
            {
                existingRecordsDict = (await _repositoryWrapper.ReportProgramExpendituresRecordsRepository
                    .GetAllAsync(new QueryOptions<ReportProgramExpendituresRecord>
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

            var categoriesValidationResult = await ValidateCategoryRulesAsync(dto, existingRecordsDict);
            if (categoriesValidationResult.IsFailed)
            {
                return categoriesValidationResult;
            }

            HandleDeletions(dto, existingRecordsDict);
            HandleUpdates(dto, existingRecordsDict);
            await HandleCreationsAsync(dto);

            int affectedRows = await _repositoryWrapper.SaveChangesAsync();
            await scope.CommitAsync(cancellationToken);

            if (affectedRows > 0)
            {
                await _mediator.Publish(new ReportFundsChangedNotification(), CancellationToken.None);
            }

            return Result.Ok(Unit.Value);
        }
        catch (DbUpdateException dbUpdateException) when (dbUpdateException.IsUniqueConstraintException())
        {
            _logger.LogWarning(
                dbUpdateException,
                "Unique constraint violated while batch saving {EntityName}.",
                nameof(ReportProgramExpendituresRecord));

            return Result.Fail<Unit>(
                ReportProgramExpendituresRecordConstants.ProgramCategoryAlreadyHasRecord());
        }
        catch (DbUpdateException dbUpdateException)
        {
            _logger.LogError(
                dbUpdateException,
                "Unexpected database failure while batch saving {EntityName}.",
                nameof(ReportProgramExpendituresRecord));

            return Result.Fail<Unit>(
                ErrorMessagesConstants.FailedToSaveEntitiesInDatabase(nameof(ReportProgramExpendituresRecord)));
        }
    }

    private static Result<Unit> ValidateExistingRecords(
        IEnumerable<long> targetIds, Dictionary<long, ReportProgramExpendituresRecord> existingRecordsDict)
    {
        var nonExistingRecordIds = targetIds
            .Where(id => !existingRecordsDict.ContainsKey(id))
            .ToList();

        if (nonExistingRecordIds.Count > 0)
        {
            return Result.Fail<Unit>(ErrorMessagesConstants.NotFound(
                nonExistingRecordIds,
                typeof(ReportProgramExpendituresRecord)));
        }

        return Result.Ok(Unit.Value);
    }

    private async Task<Result<Unit>> ValidateCategoryRulesAsync(
        BatchSaveReportProgramExpendituresRecordsDto dto,
        Dictionary<long, ReportProgramExpendituresRecord> existingRecordsDict)
    {
        var categoryIdsToValidate = dto.RecordsToCreate
                .Select(c => c.HippotherapyProgramCategoryId)
                .Concat(
                    dto.RecordsToUpdate
                        .Where(u => u.HippotherapyProgramCategoryId != existingRecordsDict[u.Id].HippotherapyProgramCategoryId)
                        .Select(u => u.HippotherapyProgramCategoryId))
                .ToList();

        if (categoryIdsToValidate.Count == 0)
        {
            return Result.Ok();
        }

        var categoriesDict = (await _repositoryWrapper.HippotherapyProgramCategoriesRepository
            .GetAllAsync(new QueryOptions<HippotherapyProgramCategory>
            {
                Filter = entity => categoryIdsToValidate.Contains(entity.Id)
            }))
            .ToDictionary(c => c.Id);

        var duplicateRecordsInCategory = await _repositoryWrapper.ReportProgramExpendituresRecordsRepository
            .GetAllAsync(new QueryOptions<ReportProgramExpendituresRecord>
            {
                Filter = entity => categoryIdsToValidate.Contains(entity.HippotherapyProgramCategoryId) &&
                                    !dto.RecordIdsToDelete.Contains(entity.Id)
            });

        return EvaluateCategoryRules(
            categoryIdsToValidate,
            categoriesDict,
            duplicateRecordsInCategory);
    }

    private static Result<Unit> EvaluateCategoryRules(
        IEnumerable<long> categoryIdsToValidate,
        Dictionary<long, HippotherapyProgramCategory> categoriesDict,
        IEnumerable<ReportProgramExpendituresRecord> duplicateRecordsInCategory)
    {
        var nonExistingCategoryIds = categoryIdsToValidate
            .Where(id => !categoriesDict.ContainsKey(id))
            .ToList();

        if (nonExistingCategoryIds.Count > 0)
        {
            return Result.Fail<Unit>(ErrorMessagesConstants.NotFound(
                nonExistingCategoryIds,
                typeof(HippotherapyProgramCategory)));
        }

        if (duplicateRecordsInCategory.Any())
        {
            var errorMessages = duplicateRecordsInCategory
                .Select(r => ReportProgramExpendituresRecordConstants.ProgramCategoryAlreadyHasRecord(r.HippotherapyProgramCategoryId));

            return Result.Fail<Unit>(errorMessages);
        }

        return Result.Ok(Unit.Value);
    }

    private void HandleDeletions(
        BatchSaveReportProgramExpendituresRecordsDto dto,
        Dictionary<long, ReportProgramExpendituresRecord> existingRecordsDict)
    {
        if (dto.RecordIdsToDelete.Count == 0)
        {
            return;
        }

        var recordsToDelete = dto.RecordIdsToDelete
            .Select(id => existingRecordsDict[id])
            .ToList();

        _repositoryWrapper.ReportProgramExpendituresRecordsRepository.DeleteRange(recordsToDelete);
    }

    private void HandleUpdates(
        BatchSaveReportProgramExpendituresRecordsDto dto,
        Dictionary<long, ReportProgramExpendituresRecord> existingRecordsDict)
    {
        if (dto.RecordsToUpdate.Count == 0)
        {
            return;
        }

        var entitiesToUpdate = new List<ReportProgramExpendituresRecord>();

        foreach (var recordToUpdateDto in dto.RecordsToUpdate)
        {
            var existingEntity = existingRecordsDict[recordToUpdateDto.Id];
            _mapper.Map(recordToUpdateDto, existingEntity);

            entitiesToUpdate.Add(existingEntity);
        }

        _repositoryWrapper.ReportProgramExpendituresRecordsRepository.UpdateRange(entitiesToUpdate);
    }

    private async Task HandleCreationsAsync(
        BatchSaveReportProgramExpendituresRecordsDto dto)
    {
        if (dto.RecordsToCreate.Count == 0)
        {
            return;
        }

        var newEntities = new List<ReportProgramExpendituresRecord>();

        foreach (var recordToCreateDto in dto.RecordsToCreate)
        {
            var entity = _mapper.Map<ReportProgramExpendituresRecord>(recordToCreateDto);
            entity.CreatedAt = DateTimeOffset.UtcNow;

            newEntities.Add(entity);
        }

        await _repositoryWrapper.ReportProgramExpendituresRecordsRepository.CreateRangeAsync(newEntities);
    }
}
