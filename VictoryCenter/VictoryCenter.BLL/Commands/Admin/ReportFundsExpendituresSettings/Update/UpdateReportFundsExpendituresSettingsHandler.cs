using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using ReportFundsExpendituresSettingsEntity = VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresSettings.Update;

public class UpdateReportFundsExpendituresSettingsHandler
    : IRequestHandler<UpdateReportFundsExpendituresSettingsCommand, Result<ReportFundsExpendituresSettingsDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly TimeProvider _timeProvider;
    private readonly IValidator<UpdateReportFundsExpendituresSettingsCommand> _validator;

    public UpdateReportFundsExpendituresSettingsHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateReportFundsExpendituresSettingsCommand> validator,
        TimeProvider timeProvider)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _timeProvider = timeProvider;
    }

    public async Task<Result<ReportFundsExpendituresSettingsDto>> Handle(
        UpdateReportFundsExpendituresSettingsCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var settingsResult = await ReportFundsExpendituresSettingsHelper.GetOrCreateSettingsAsync(_repositoryWrapper, _timeProvider);
            if (settingsResult.IsFailed)
            {
                return Result.Fail<ReportFundsExpendituresSettingsDto>(settingsResult.Errors);
            }

            var entityToUpdate = settingsResult.Value;
            var disclaimerChanged = entityToUpdate.DisclaimerTitle != request.UpdateReportFundsExpendituresSettingsDto.DisclaimerTitle;

            _mapper.Map(request.UpdateReportFundsExpendituresSettingsDto, entityToUpdate);
            _repositoryWrapper.ReportFundsExpendituresSettingsRepository.Update(entityToUpdate);

            if (disclaimerChanged)
            {
                var localizations = (await _repositoryWrapper.ReportFundsExpendituresSettingsLocalizationsRepository
                    .GetAllAsync(new QueryOptions<ReportFundsExpendituresSettingsLocalization>
                    {
                        Filter = l => l.EntityId == entityToUpdate.Id,
                        AsNoTracking = false
                    })).ToList();

                foreach (var loc in localizations)
                {
                    loc.TranslationStatus = TranslationStatus.Outdated;
                }

                if (localizations.Count > 0)
                {
                    _repositoryWrapper.ReportFundsExpendituresSettingsLocalizationsRepository.UpdateRange(localizations);
                }
            }

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
