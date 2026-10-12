using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;
using VictoryCenter.BLL.Interfaces.HippotherapyLandingPages;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using HippotherapyLandingPageAdvantagesSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageAdvantagesSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAdvantagesSection.Create;

public class CreateHippotherapyLandingPageAdvantagesSectionLocalizationHandler
    : IRequestHandler<CreateHippotherapyLandingPageAdvantagesSectionLocalizationCommand, Result<HippotherapyLandingPageAdvantagesSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HippotherapyLandingPageAdvantagesSectionEntity, HippotherapyLandingPageAdvantagesSectionLocalization> _localizationService;
    private readonly IValidator<CreateHippotherapyLandingPageAdvantagesSectionLocalizationCommand> _validator;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IHippotherapyLandingPageAdvantageCardLocalizationUpdater _cardsUpdater;

    public CreateHippotherapyLandingPageAdvantagesSectionLocalizationHandler(
        IMapper mapper,
        ILocalizationService<HippotherapyLandingPageAdvantagesSectionEntity, HippotherapyLandingPageAdvantagesSectionLocalization> localizationService,
        IValidator<CreateHippotherapyLandingPageAdvantagesSectionLocalizationCommand> validator,
        IRepositoryWrapper repositoryWrapper,
        IHippotherapyLandingPageAdvantageCardLocalizationUpdater cardsUpdater)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
        _repositoryWrapper = repositoryWrapper;
        _cardsUpdater = cardsUpdater;
    }

    public async Task<Result<HippotherapyLandingPageAdvantagesSectionLocalizationDto>> Handle(
        CreateHippotherapyLandingPageAdvantagesSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var dto = request.CreateHippotherapyLandingPageAdvantagesSectionLocalizationDto;

            var section = await GetSectionWithCardsAsync(dto.EntityId);
            if (section is null)
            {
                return Result.Fail<HippotherapyLandingPageAdvantagesSectionLocalizationDto>(
                    ErrorMessagesConstants.NotFound(dto.EntityId, typeof(HippotherapyLandingPageAdvantagesSectionEntity)));
            }

            using var transaction = _repositoryWrapper.BeginTransaction();

            var entity = _mapper.Map<HippotherapyLandingPageAdvantagesSectionLocalization>(dto);
            var result = await _localizationService.CreateEntityLocalizationAsync(entity);

            var cardsResult = await _cardsUpdater.UpsertCardsAsync(section, dto.Cards, dto.LanguageId);
            if (cardsResult.IsFailed)
            {
                return Result.Fail<HippotherapyLandingPageAdvantagesSectionLocalizationDto>(cardsResult.Errors);
            }

            transaction.Complete();

            var responseDto = _mapper.Map<HippotherapyLandingPageAdvantagesSectionLocalizationDto>(result);
            responseDto.Cards = cardsResult.Value;
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<HippotherapyLandingPageAdvantagesSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<HippotherapyLandingPageAdvantagesSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyLandingPageAdvantagesSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyLandingPageAdvantagesSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyLandingPageAdvantagesSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyLandingPageAdvantagesSectionLocalization)));
        }
    }

    private Task<HippotherapyLandingPageAdvantagesSectionEntity?> GetSectionWithCardsAsync(long sectionId)
    {
        return _repositoryWrapper.HippotherapyLandingPageAdvantagesSectionsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<HippotherapyLandingPageAdvantagesSectionEntity>
            {
                Filter = s => s.Id == sectionId,
                Include = q => q.Include(s => s.AdvantageCards)
            });
    }
}
