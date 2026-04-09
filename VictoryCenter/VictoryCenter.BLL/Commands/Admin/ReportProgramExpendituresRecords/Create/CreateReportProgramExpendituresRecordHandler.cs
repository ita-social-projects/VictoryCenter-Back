using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Create;

public class CreateReportProgramExpendituresRecordHandler :
    IRequestHandler<CreateReportProgramExpendituresRecordCommand, Result<ReportProgramExpendituresRecordDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateReportProgramExpendituresRecordCommand> _validator;

    public CreateReportProgramExpendituresRecordHandler(
        IValidator<CreateReportProgramExpendituresRecordCommand> validator,
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper)
    {
        _validator = validator;
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<ReportProgramExpendituresRecordDto>> Handle(
        CreateReportProgramExpendituresRecordCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var programCategory =
                await _repositoryWrapper.ReportProgramExpendituresCategoriesRepository.GetFirstOrDefaultAsync(
                    new QueryOptions<ReportProgramExpendituresCategory>
                    {
                        Filter = category =>
                            category.Id == request.CreateReportProgramExpendituresRecordDto.ProgramCategoryId
                    });

            if (programCategory is null)
            {
                return Result.Fail(
                    ErrorMessagesConstants.NotFound(
                        request.CreateReportProgramExpendituresRecordDto.ProgramCategoryId,
                        typeof(ReportProgramExpendituresCategory)));
            }

            if (await DuplicateRecordExistsAsync(request.CreateReportProgramExpendituresRecordDto.ProgramCategoryId))
            {
                return Result.Fail(ReportProgramExpendituresRecordConstants.ProgramCategoryAlreadyHasRecord);
            }

            var reportProgramExpendituresRecord =
                _mapper.Map<ReportProgramExpendituresRecord>(request.CreateReportProgramExpendituresRecordDto);

            await _repositoryWrapper.ReportProgramExpendituresRecordsRepository.CreateAsync(
                reportProgramExpendituresRecord);

            reportProgramExpendituresRecord.CreatedAt = DateTimeOffset.UtcNow;

            if (await _repositoryWrapper.SaveChangesAsync() == 0)
            {
                return Result.Fail(
                    ErrorMessagesConstants.FailedToCreateEntity(typeof(ReportProgramExpendituresRecord)));
            }

            return Result.Ok(_mapper.Map<ReportProgramExpendituresRecordDto>(reportProgramExpendituresRecord));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<ReportProgramExpendituresRecordDto>(ex.Message);
        }
        catch (DbUpdateException)
        {
            return Result.Fail<ReportProgramExpendituresRecordDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(ReportProgramExpendituresRecord)));
        }
    }

    private async Task<bool> DuplicateRecordExistsAsync(long programCategoryId)
    {
        return await _repositoryWrapper.ReportProgramExpendituresRecordsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<ReportProgramExpendituresRecord>
            {
                Filter = record => record.ProgramCategoryId == programCategoryId
            }) is not null;
    }
}
