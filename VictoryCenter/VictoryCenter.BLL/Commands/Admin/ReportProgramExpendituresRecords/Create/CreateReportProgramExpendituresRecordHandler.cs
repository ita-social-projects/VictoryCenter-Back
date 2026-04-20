using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Create;

public class CreateReportProgramExpendituresRecordHandler :
    IRequestHandler<CreateReportProgramExpendituresRecordCommand, Result<ReportProgramExpendituresRecordDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public CreateReportProgramExpendituresRecordHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
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

        if (await DuplicateRecordExistsAsync(reportProgramExpendituresRecord))
        {
            return Result.Fail(ReportProgramExpendituresRecordConstants
                .ProgramCategoryAlreadyHasRecordForSpecifiedYear(
                    request.CreateReportProgramExpendituresRecordDto.HippotherapyProgramCategoryId,
                    request.CreateReportProgramExpendituresRecordDto.ReportingYear));
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
        }
        catch (DbUpdateException dbUpdateException) when (dbUpdateException.IsUniqueConstraintException())
        {
            return Result.Fail<ReportProgramExpendituresRecordDto>(
                ReportProgramExpendituresRecordConstants
                    .ProgramCategoryAlreadyHasRecordForSpecifiedYear(
                        request.CreateReportProgramExpendituresRecordDto.HippotherapyProgramCategoryId,
                        request.CreateReportProgramExpendituresRecordDto.ReportingYear));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<ReportProgramExpendituresRecordDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(
                    typeof(ReportProgramExpendituresRecord)));
        }

        return Result.Ok(_mapper.Map<ReportProgramExpendituresRecordDto>(reportProgramExpendituresRecord));
    }

    private async Task<bool> DuplicateRecordExistsAsync(ReportProgramExpendituresRecord record)
    {
        return await _repositoryWrapper.ReportProgramExpendituresRecordsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<ReportProgramExpendituresRecord>
            {
                Filter = recordFromDatabase =>
                    recordFromDatabase.HippotherapyProgramCategoryId == record.HippotherapyProgramCategoryId &&
                    recordFromDatabase.ReportingYear == record.ReportingYear
            }) is not null;
    }
}
