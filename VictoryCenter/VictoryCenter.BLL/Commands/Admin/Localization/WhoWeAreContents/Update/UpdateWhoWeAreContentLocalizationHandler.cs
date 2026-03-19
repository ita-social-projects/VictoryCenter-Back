using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.WhoWeAreContents.Update;

public class UpdateWhoWeAreContentLocalizationHandler : IRequestHandler<UpdateWhoWeAreContentLocalizationCommand, Result<List<WhoWeAreContentLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateWhoWeAreContentLocalizationCommand> _validator;
    private readonly ILocalizationService<WhoWeAreContent, WhoWeAreContentLocalization> _localizationService;
    private readonly IRepositoryWrapper _repository;

    public UpdateWhoWeAreContentLocalizationHandler(
        IMapper mapper,
        IValidator<UpdateWhoWeAreContentLocalizationCommand> validator,
        ILocalizationService<WhoWeAreContent, WhoWeAreContentLocalization> localizationService,
        IRepositoryWrapper repository)
    {
        _mapper = mapper;
        _validator = validator;
        _localizationService = localizationService;
        _repository = repository;
    }

    public async Task<Result<List<WhoWeAreContentLocalizationDto>>> Handle(UpdateWhoWeAreContentLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using var transaction = _repository.BeginTransaction();

            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var sanitizedDtosResult = await WhoWeAreContentLocalizationHandlerHelper.ValidateAndSanitizeAsync(
                _repository,
                request.SectionType,
                request.ContentLocalizationDtos);

            if (sanitizedDtosResult.IsFailed)
            {
                return Result.Fail<List<WhoWeAreContentLocalizationDto>>(sanitizedDtosResult.Errors.Select(e => e.Message));
            }

            var localizationsToUpdate = sanitizedDtosResult.Value
                .Select(sanitizedDto => _mapper.Map<WhoWeAreContentLocalization>(sanitizedDto))
                .ToList();

            await _localizationService.TrackEntityLocalizationAsync(localizationsToUpdate, true);

            if (await _repository.SaveChangesAsync() <= 0)
            {
                return Result.Fail<List<WhoWeAreContentLocalizationDto>>(
                    ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(WhoWeAreContentLocalization)));
            }

            var languageId = request.ContentLocalizationDtos.First().LanguageId;
            var entityIds = request.ContentLocalizationDtos.Select(x => x.EntityId).ToList();

            var updatedLocalizations = await _repository.GetRepository<WhoWeAreContentLocalization>()
                .GetAllAsync(new QueryOptions<WhoWeAreContentLocalization>
                {
                    Filter = l => entityIds.Contains(l.EntityId) && l.LanguageId == languageId,
                    Include = l => l.Include(x => x.Language)
                });

            var response = _mapper.Map<List<WhoWeAreContentLocalizationDto>>(updatedLocalizations);

            transaction.Complete();
            return Result.Ok(response);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<List<WhoWeAreContentLocalizationDto>>(knfex.Message);
        }
        catch (ArgumentException aex)
        {
            return Result.Fail<List<WhoWeAreContentLocalizationDto>>(aex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<List<WhoWeAreContentLocalizationDto>>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(WhoWeAreContentLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<List<WhoWeAreContentLocalizationDto>>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<List<WhoWeAreContentLocalizationDto>>(ErrorMessagesConstants.
                FailedToUpdateEntityInDatabase(typeof(WhoWeAreContentLocalization)));
        }
    }
}
