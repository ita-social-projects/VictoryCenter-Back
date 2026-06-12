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

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Update;

public class UpdateReportFundsExpendituresRecordHandler
    : IRequestHandler<UpdateReportFundsExpendituresRecordCommand, Result<ReportFundsExpendituresRecordDto>>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateReportFundsExpendituresRecordCommand> _validator;

    public UpdateReportFundsExpendituresRecordHandler(
        IMapper mapper,
        IMediator mediator,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateReportFundsExpendituresRecordCommand> validator)
    {
        _mapper = mapper;
        _mediator = mediator;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<ReportFundsExpendituresRecordDto>> Handle(
        UpdateReportFundsExpendituresRecordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var entityToUpdate = await _repositoryWrapper.ReportFundsExpendituresRecordsRepository
                .GetFirstOrDefaultAsync(new QueryOptions<ReportFundsExpendituresRecord>
                {
                    Filter = entity => entity.Id == request.Id
                });

            if (entityToUpdate is null)
            {
                return Result.Fail<ReportFundsExpendituresRecordDto>(
                    ErrorMessagesConstants.NotFound(request.Id, typeof(ReportFundsExpendituresRecord)));
            }

            if (entityToUpdate.CategoryId != request.UpdateReportFundsExpendituresRecordDto.CategoryId)
            {
                var category = await _repositoryWrapper.ReportFundsExpendituresCategoriesRepository
                    .GetFirstOrDefaultAsync(new QueryOptions<ReportFundsExpendituresCategory>
                    {
                        Filter = entity => entity.Id == request.UpdateReportFundsExpendituresRecordDto.CategoryId
                    });

                if (category is null)
                {
                    return Result.Fail<ReportFundsExpendituresRecordDto>(
                        ErrorMessagesConstants.NotFound(
                            request.UpdateReportFundsExpendituresRecordDto.CategoryId,
                            typeof(ReportFundsExpendituresCategory)));
                }

                if (category.Type != entityToUpdate.Type)
                {
                    return Result.Fail<ReportFundsExpendituresRecordDto>(
                        ReportFundsExpendituresRecordConstants.CategoryTypeMustMatchRecordType);
                }

                var duplicateRecordInCategory = await _repositoryWrapper.ReportFundsExpendituresRecordsRepository
                    .GetFirstOrDefaultAsync(new QueryOptions<ReportFundsExpendituresRecord>
                    {
                        Filter = entity =>
                            entity.CategoryId == request.UpdateReportFundsExpendituresRecordDto.CategoryId &&
                            entity.Id != request.Id
                    });

                if (duplicateRecordInCategory is not null)
                {
                    return Result.Fail<ReportFundsExpendituresRecordDto>(
                        ReportFundsExpendituresRecordConstants.CategoryAlreadyHasRecord);
                }
            }

            _mapper.Map(request.UpdateReportFundsExpendituresRecordDto, entityToUpdate);
            _repositoryWrapper.ReportFundsExpendituresRecordsRepository.Update(entityToUpdate);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                await _mediator.Publish(new ReportFundsChangedNotification(), CancellationToken.None);

                return Result.Ok(_mapper.Map<ReportFundsExpendituresRecordDto>(entityToUpdate));
            }

            return Result.Fail<ReportFundsExpendituresRecordDto>(
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(ReportFundsExpendituresRecord)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<ReportFundsExpendituresRecordDto>(ex.Message);
        }
        catch (DbUpdateException)
        {
            return Result.Fail<ReportFundsExpendituresRecordDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(ReportFundsExpendituresRecord)));
        }
    }
}
