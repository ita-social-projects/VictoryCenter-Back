using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.Notifications.ReportFunds;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Create;

public class CreateReportFundsExpendituresRecordHandler
    : IRequestHandler<CreateReportFundsExpendituresRecordCommand, Result<ReportFundsExpendituresRecordDto>>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateReportFundsExpendituresRecordCommand> _validator;

    public CreateReportFundsExpendituresRecordHandler(
        IMapper mapper,
        IMediator mediator,
        IRepositoryWrapper repositoryWrapper,
        IValidator<CreateReportFundsExpendituresRecordCommand> validator)
    {
        _mapper = mapper;
        _mediator = mediator;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<ReportFundsExpendituresRecordDto>> Handle(
        CreateReportFundsExpendituresRecordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var category = await _repositoryWrapper.ReportFundsExpendituresCategoriesRepository
                .GetFirstOrDefaultAsync(new QueryOptions<ReportFundsExpendituresCategory>
                {
                    Filter = entity => entity.Id == request.CreateReportFundsExpendituresRecordDto.CategoryId
                });

            if (category is null)
            {
                return Result.Fail<ReportFundsExpendituresRecordDto>(
                    ErrorMessagesConstants.NotFound(
                        request.CreateReportFundsExpendituresRecordDto.CategoryId,
                        typeof(ReportFundsExpendituresCategory)));
            }

            if (category.Type != request.CreateReportFundsExpendituresRecordDto.Type)
            {
                return Result.Fail<ReportFundsExpendituresRecordDto>(
                    ReportFundsExpendituresRecordConstants.CategoryTypeMustMatchRecordType);
            }

            var duplicateRecord = await _repositoryWrapper.ReportFundsExpendituresRecordsRepository
                .GetFirstOrDefaultAsync(new QueryOptions<ReportFundsExpendituresRecord>
                {
                    Filter = entity => entity.CategoryId == request.CreateReportFundsExpendituresRecordDto.CategoryId
                });

            if (duplicateRecord is not null)
            {
                return Result.Fail<ReportFundsExpendituresRecordDto>(
                    ReportFundsExpendituresRecordConstants.CategoryAlreadyHasRecord);
            }

            var entity = _mapper.Map<ReportFundsExpendituresRecord>(request.CreateReportFundsExpendituresRecordDto);
            entity.CreatedAt = DateTimeOffset.UtcNow;

            await _repositoryWrapper.ReportFundsExpendituresRecordsRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                await _mediator.Publish(new ReportFundsChangedNotification(), CancellationToken.None);

                return Result.Ok(_mapper.Map<ReportFundsExpendituresRecordDto>(entity));
            }

            return Result.Fail<ReportFundsExpendituresRecordDto>(
                ErrorMessagesConstants.FailedToCreateEntity(typeof(ReportFundsExpendituresRecord)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<ReportFundsExpendituresRecordDto>(ex.Message);
        }
        catch (DbUpdateException)
        {
            return Result.Fail<ReportFundsExpendituresRecordDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(ReportFundsExpendituresRecord)));
        }
    }
}
