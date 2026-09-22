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
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Update;

public class UpdateReportFundsExpendituresRecordHandler
    : IRequestHandler<UpdateReportFundsExpendituresRecordCommand, Result<ReportFundsExpendituresRecordDto>>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateReportFundsExpendituresRecordCommand> _validator;
    private readonly IReportFundsExpendituresRecordHelper _helper;

    public UpdateReportFundsExpendituresRecordHandler(
        IMapper mapper,
        IMediator mediator,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateReportFundsExpendituresRecordCommand> validator,
        IReportFundsExpendituresRecordHelper helper)
    {
        _mapper = mapper;
        _mediator = mediator;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _helper = helper;
    }

    public async Task<Result<ReportFundsExpendituresRecordDto>> Handle(
        UpdateReportFundsExpendituresRecordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var settingsResult = await _helper.GetAndValidateSettingsAsync();
            if (settingsResult.IsFailed)
            {
                return Result.Fail<ReportFundsExpendituresRecordDto>(settingsResult.Errors);
            }

            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var entityToUpdate = await _repositoryWrapper.ReportFundsExpendituresRecordsRepository
                .GetFirstOrDefaultAsync(new QueryOptions<ReportFundsExpendituresRecord>
                {
                    Filter = entity => entity.Id == request.Id
                });

            if (entityToUpdate is null)
            {
                return Result.Fail<ReportFundsExpendituresRecordDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(ReportFundsExpendituresRecord)));
            }

            var categoryValidationResult = await _helper.ValidateCategoryChangeAsync(entityToUpdate, request.UpdateReportFundsExpendituresRecordDto.CategoryId, request.Id);

            if (categoryValidationResult.IsFailed)
            {
                return Result.Fail<ReportFundsExpendituresRecordDto>(categoryValidationResult.Errors);
            }

            _mapper.Map(request.UpdateReportFundsExpendituresRecordDto, entityToUpdate);

            var (amountUah, amountUsd) = _helper.CalculateAmounts(
                request.UpdateReportFundsExpendituresRecordDto.Amount!.Value,
                request.UpdateReportFundsExpendituresRecordDto.Currency,
                settingsResult.Value.ExchangeRate);
            entityToUpdate.AmountUah = amountUah;
            entityToUpdate.AmountUsd = amountUsd;

            _repositoryWrapper.ReportFundsExpendituresRecordsRepository.Update(entityToUpdate);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                await _mediator.Publish(new ReportFundsChangedNotification(), CancellationToken.None);
                return Result.Ok(_mapper.Map<ReportFundsExpendituresRecordDto>(entityToUpdate));
            }

            return Result.Fail<ReportFundsExpendituresRecordDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(ReportFundsExpendituresRecord)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<ReportFundsExpendituresRecordDto>(ex.Message);
        }
        catch (DbUpdateException)
        {
            return Result.Fail<ReportFundsExpendituresRecordDto>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(ReportFundsExpendituresRecord)));
        }
    }
}
