using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;
using VictoryCenter.BLL.Interfaces.HippotherapyLandingPages;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using HippotherapyLandingPageParticipantsSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageParticipantsSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageParticipantsSection.Update;

public class UpdateHippotherapyLandingPageParticipantsSectionLocalizationHandler
    : IRequestHandler<UpdateHippotherapyLandingPageParticipantsSectionLocalizationCommand, Result<HippotherapyLandingPageParticipantsSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HippotherapyLandingPageParticipantsSectionEntity, HippotherapyLandingPageParticipantsSectionLocalization> _localizationService;
    private readonly IValidator<UpdateHippotherapyLandingPageParticipantsSectionLocalizationCommand> _validator;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IHippotherapyLandingPageParticipantCardLocalizationUpdater _cardsUpdater;

    public UpdateHippotherapyLandingPageParticipantsSectionLocalizationHandler(
        IMapper mapper,
        ILocalizationService<HippotherapyLandingPageParticipantsSectionEntity, HippotherapyLandingPageParticipantsSectionLocalization> localizationService,
        IValidator<UpdateHippotherapyLandingPageParticipantsSectionLocalizationCommand> validator,
        IRepositoryWrapper repositoryWrapper,
        IHippotherapyLandingPageParticipantCardLocalizationUpdater cardsUpdater)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
        _repositoryWrapper = repositoryWrapper;
        _cardsUpdater = cardsUpdater;
    }

    public async Task<Result<HippotherapyLandingPageParticipantsSectionLocalizationDto>> Handle(
        UpdateHippotherapyLandingPageParticipantsSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var dto = request.UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto;

            var section = await GetSectionWithCardsAsync(request.EntityId);
            if (section is null)
            {
                return Result.Fail<HippotherapyLandingPageParticipantsSectionLocalizationDto>(
                    ErrorMessagesConstants.NotFound(request.EntityId, typeof(HippotherapyLandingPageParticipantsSectionEntity)));
            }

            using var transaction = _repositoryWrapper.BeginTransaction();

            var entity = _mapper.Map<HippotherapyLandingPageParticipantsSectionLocalization>(dto);
            entity.EntityId = request.EntityId;
            entity.LanguageId = request.LanguageId;
            var result = await _localizationService.UpdateEntityLocalizationAsync(entity);

            var cardsResult = await _cardsUpdater.UpsertCardsAsync(section, dto.Cards, request.LanguageId);
            if (cardsResult.IsFailed)
            {
                return Result.Fail<HippotherapyLandingPageParticipantsSectionLocalizationDto>(cardsResult.Errors);
            }

            transaction.Complete();

            var responseDto = _mapper.Map<HippotherapyLandingPageParticipantsSectionLocalizationDto>(result);
            responseDto.Cards = cardsResult.Value;
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<HippotherapyLandingPageParticipantsSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<HippotherapyLandingPageParticipantsSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyLandingPageParticipantsSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyLandingPageParticipantsSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyLandingPageParticipantsSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HippotherapyLandingPageParticipantsSectionLocalization)));
        }
    }

    private Task<HippotherapyLandingPageParticipantsSectionEntity?> GetSectionWithCardsAsync(long sectionId)
    {
        return _repositoryWrapper.HippotherapyLandingPageParticipantsSectionsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<HippotherapyLandingPageParticipantsSectionEntity>
            {
                Filter = s => s.Id == sectionId,
                Include = q => q.Include(s => s.ParticipantCards)
            });
    }
}
