using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using ReportFundsExpendituresSettingsEntity = VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresSettings.Update;

public class UpdateReportFundsExpendituresSettingsHandler
    : IRequestHandler<UpdateReportFundsExpendituresSettingsCommand, Result<ReportFundsExpendituresSettingsDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateReportFundsExpendituresSettingsCommand> _validator;

    public UpdateReportFundsExpendituresSettingsHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateReportFundsExpendituresSettingsCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<ReportFundsExpendituresSettingsDto>> Handle(
        UpdateReportFundsExpendituresSettingsCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var entityToUpdate = await _repositoryWrapper.ReportFundsExpendituresSettingsRepository
                .GetFirstOrDefaultAsync(new QueryOptions<ReportFundsExpendituresSettingsEntity>
                {
                    Filter = settings => settings.Id == ReportFundsExpendituresSettingsConstants.SingletonSettingsId
                });

            if (entityToUpdate is null)
            {
                return Result.Fail<ReportFundsExpendituresSettingsDto>(
                    ErrorMessagesConstants.NotFound(
                        ReportFundsExpendituresSettingsConstants.SingletonSettingsId,
                        typeof(ReportFundsExpendituresSettingsEntity)));
            }

            _mapper.Map(request.UpdateReportFundsExpendituresSettingsDto, entityToUpdate);
            _repositoryWrapper.ReportFundsExpendituresSettingsRepository.Update(entityToUpdate);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(_mapper.Map<ReportFundsExpendituresSettingsDto>(entityToUpdate));
            }

            return Result.Fail<ReportFundsExpendituresSettingsDto>(
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(ReportFundsExpendituresSettingsEntity)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<ReportFundsExpendituresSettingsDto>(ex.Message);
        }
        catch (DbUpdateException)
        {
            return Result.Fail<ReportFundsExpendituresSettingsDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(ReportFundsExpendituresSettingsEntity)));
        }
    }
}
