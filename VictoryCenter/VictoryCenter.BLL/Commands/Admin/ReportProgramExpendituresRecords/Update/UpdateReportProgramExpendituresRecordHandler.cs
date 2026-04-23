using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Update;

public class UpdateReportProgramExpendituresRecordHandler
    : IRequestHandler<UpdateReportProgramExpendituresRecordCommand, Result<ReportProgramExpendituresRecordDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateReportProgramExpendituresRecordCommand> _validator;

    public UpdateReportProgramExpendituresRecordHandler(
        IValidator<UpdateReportProgramExpendituresRecordCommand> validator,
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper)
    {
        _validator = validator;
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<ReportProgramExpendituresRecordDto>> Handle(
        UpdateReportProgramExpendituresRecordCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.Select(error => error.ErrorMessage));
        }

        var recordToUpdate = await _repositoryWrapper
            .ReportProgramExpendituresRecordsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<ReportProgramExpendituresRecord>
            {
                Filter = e => e.Id == request.ReportProgramExpendituresRecordId
            });

        if (recordToUpdate is null)
        {
            return Result.Fail(
                ErrorMessagesConstants.NotFound(
                    request.ReportProgramExpendituresRecordId,
                    typeof(ReportProgramExpendituresRecord)));
        }

        var recordWasMovedToDifferentCategory =
            request.Dto.HippotherapyProgramCategoryId != recordToUpdate.HippotherapyProgramCategoryId;

        _mapper.Map(request.Dto, recordToUpdate);

        if (recordWasMovedToDifferentCategory)
        {
            var recordWithinSameCategoryWithSameYearExists = await _repositoryWrapper
                .ReportProgramExpendituresRecordsRepository
                .RecordWithinSameCategoryWithSameYearExistsAsync(recordToUpdate);

            if (recordWithinSameCategoryWithSameYearExists)
            {
                return Result.Fail(ReportProgramExpendituresRecordConstants
                    .ProgramCategoryAlreadyHasRecordForSpecifiedYear(
                        recordToUpdate.HippotherapyProgramCategoryId,
                        recordToUpdate.ReportingYear));
            }
        }

        _repositoryWrapper.ReportProgramExpendituresRecordsRepository.Update(recordToUpdate);

        try
        {
            var saveChangesResult = await _repositoryWrapper.SaveChangesAsync();
            if (saveChangesResult == 0)
            {
                return Result.Fail(
                    ErrorMessagesConstants.FailedToUpdateEntity(typeof(ReportProgramExpendituresRecord)));
            }
        }
        catch (DbUpdateException dbUpdateException) when (dbUpdateException.IsUniqueConstraintException())
        {
            return Result.Fail<ReportProgramExpendituresRecordDto>(
                ReportProgramExpendituresRecordConstants
                    .ProgramCategoryAlreadyHasRecordForSpecifiedYear(
                        recordToUpdate.HippotherapyProgramCategoryId,
                        recordToUpdate.ReportingYear));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<ReportProgramExpendituresRecordDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(
                    typeof(ReportProgramExpendituresRecord)));
        }

        return Result.Ok(_mapper.Map<ReportProgramExpendituresRecordDto>(recordToUpdate));
    }
}
