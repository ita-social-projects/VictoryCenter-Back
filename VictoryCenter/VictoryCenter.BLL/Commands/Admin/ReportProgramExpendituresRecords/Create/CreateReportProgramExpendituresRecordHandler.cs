using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Notifications.ReportFunds;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Create;

public class CreateReportProgramExpendituresRecordHandler :
    IRequestHandler<CreateReportProgramExpendituresRecordCommand, Result<ReportProgramExpendituresRecordDto>>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public CreateReportProgramExpendituresRecordHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IMediator mediator)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<Result<ReportProgramExpendituresRecordDto>> Handle(
        CreateReportProgramExpendituresRecordCommand request,
        CancellationToken cancellationToken)
    {
        var programCategory =
            await _repositoryWrapper.HippotherapyProgramCategoriesRepository.GetFirstOrDefaultAsync(
                new QueryOptions<HippotherapyProgramCategory>
                {
                    Filter = category =>
                        category.Id == request.CreateReportProgramExpendituresRecordDto
                            .HippotherapyProgramCategoryId
                });

        if (programCategory is null)
        {
            return Result.Fail(
                ErrorMessagesConstants.NotFound(
                    request.CreateReportProgramExpendituresRecordDto.HippotherapyProgramCategoryId,
                    typeof(HippotherapyProgramCategory)));
        }

        var reportProgramExpendituresRecord =
            _mapper.Map<ReportProgramExpendituresRecord>(request.CreateReportProgramExpendituresRecordDto);

        var recordWithinSameCategoryExists = await _repositoryWrapper
            .ReportProgramExpendituresRecordsRepository
            .RecordWithinSameCategoryExistsAsync(reportProgramExpendituresRecord);

        if (recordWithinSameCategoryExists)
        {
            return Result.Fail(ReportProgramExpendituresRecordConstants
                .ProgramCategoryAlreadyHasRecord(
                    request.CreateReportProgramExpendituresRecordDto.HippotherapyProgramCategoryId));
        }

        reportProgramExpendituresRecord.CreatedAt = DateTimeOffset.UtcNow;

        await _repositoryWrapper.ReportProgramExpendituresRecordsRepository.CreateAsync(
            reportProgramExpendituresRecord);

        try
        {
            if (await _repositoryWrapper.SaveChangesAsync() == 0)
            {
                return Result.Fail(
                    ErrorMessagesConstants.FailedToCreateEntity(typeof(ReportProgramExpendituresRecord)));
            }

            await _mediator.Publish(new ReportFundsChangedNotification(), CancellationToken.None);
        }
        catch (DbUpdateException dbUpdateException) when (dbUpdateException.IsUniqueConstraintException())
        {
            return Result.Fail<ReportProgramExpendituresRecordDto>(
                ReportProgramExpendituresRecordConstants
                    .ProgramCategoryAlreadyHasRecord(
                        request.CreateReportProgramExpendituresRecordDto.HippotherapyProgramCategoryId));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<ReportProgramExpendituresRecordDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(
                    typeof(ReportProgramExpendituresRecord)));
        }

        return Result.Ok(_mapper.Map<ReportProgramExpendituresRecordDto>(reportProgramExpendituresRecord));
    }
}
