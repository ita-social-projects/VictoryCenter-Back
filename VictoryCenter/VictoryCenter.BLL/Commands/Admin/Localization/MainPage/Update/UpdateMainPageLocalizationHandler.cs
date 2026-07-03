using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.BLL.Commands.Admin.Localization.MainPage.Update;

public class UpdateMainPageLocalizationHandler : IRequestHandler<UpdateMainPageLocalizationCommand, Result<MainPageLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateMainPageLocalizationCommand> _validator;
    private readonly ILocalizationService<MainPageEntity, MainPageLocalization> _mainPageLocalizationService;
    private readonly ILocalizationService<MainAboutUs, MainAboutUsLocalization> _mainAboutUsLocalizationService;
    private readonly ILocalizationService<MainPartners, MainPartnersLocalization> _mainPartnersLocalizationService;
    private readonly ILocalizationService<MainDonations, MainDonationsLocalization> _mainDonationsLocalizationService;

    public UpdateMainPageLocalizationHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateMainPageLocalizationCommand> validator,
        ILocalizationService<MainPageEntity, MainPageLocalization> mainPageLocalizationService,
        ILocalizationService<MainAboutUs, MainAboutUsLocalization> mainAboutUsLocalizationService,
        ILocalizationService<MainPartners, MainPartnersLocalization> mainPartnersLocalizationService,
        ILocalizationService<MainDonations, MainDonationsLocalization> mainDonationsLocalizationService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _mainPageLocalizationService = mainPageLocalizationService;
        _mainAboutUsLocalizationService = mainAboutUsLocalizationService;
        _mainPartnersLocalizationService = mainPartnersLocalizationService;
        _mainDonationsLocalizationService = mainDonationsLocalizationService;
    }

    public async Task<Result<MainPageLocalizationDto>> Handle(UpdateMainPageLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            using var transaction = _repositoryWrapper.BeginTransaction();

            var mainPage = await _repositoryWrapper.MainPageRepository
                .GetFirstOrDefaultAsync(new QueryOptions<MainPageEntity>
                {
                    Filter = e => e.Id == request.EntityId,
                    Include = query => query
                        .Include(e => e.MainAboutUs)
                        .Include(e => e.MainPartners)
                        .Include(e => e.MainDonations)
                });

            if (mainPage is null)
            {
                return Result.Fail<MainPageLocalizationDto>(ErrorMessagesConstants.NotFound());
            }

            var dto = request.Dto;

            if (!ValidateBlocksExist(dto, mainPage))
            {
                return Result.Fail<MainPageLocalizationDto>(ErrorMessagesConstants.NotFound());
            }

            MainPageLocalization mainPageLocalizationEntity = _mapper.Map<MainPageLocalization>(dto);
            mainPageLocalizationEntity.EntityId = request.EntityId;
            mainPageLocalizationEntity.LanguageId = request.LanguageId;
            var updatedMainPageLocalization = await _mainPageLocalizationService.UpdateEntityLocalizationAsync(mainPageLocalizationEntity);

            var (mainAboutUsDto, mainPartnersDto, mainDonationsDto) =
                await UpdateBlockLocalizationsAsync(dto, mainPage, request.LanguageId);

            transaction.Complete();

            var response = _mapper.Map<MainPageLocalizationDto>(updatedMainPageLocalization) with
            {
                MainAboutUs = mainAboutUsDto,
                MainPartners = mainPartnersDto,
                MainDonations = mainDonationsDto
            };

            return Result.Ok(response);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<MainPageLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<MainPageLocalizationDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(MainPageLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<MainPageLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<MainPageLocalizationDto>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(MainPageLocalization)));
        }
    }

    private static bool ValidateBlocksExist(UpdateMainPageLocalizationDto dto, MainPageEntity mainPage)
    {
        if (dto.MainAboutUs is not null && mainPage.MainAboutUs is null)
        {
            return false;
        }

        if (dto.MainPartners is not null && mainPage.MainPartners is null)
        {
            return false;
        }

        if (dto.MainDonations is not null && mainPage.MainDonations is null)
        {
            return false;
        }

        return true;
    }

    private async Task<(MainAboutUsLocalizationDto?, MainPartnersLocalizationDto?, MainDonationsLocalizationDto?)>
        UpdateBlockLocalizationsAsync(UpdateMainPageLocalizationDto dto, MainPageEntity mainPage, long languageId)
    {
        MainAboutUsLocalizationDto? mainAboutUsDto = null;
        if (dto.MainAboutUs is not null)
        {
            var entity = _mapper.Map<MainAboutUsLocalization>(dto.MainAboutUs);
            entity.EntityId = mainPage.MainAboutUs!.Id;
            entity.LanguageId = languageId;
            mainAboutUsDto = _mapper.Map<MainAboutUsLocalizationDto>(await UpsertLocalizationAsync(entity, _mainAboutUsLocalizationService));
        }

        MainPartnersLocalizationDto? mainPartnersDto = null;
        if (dto.MainPartners is not null)
        {
            var entity = _mapper.Map<MainPartnersLocalization>(dto.MainPartners);
            entity.EntityId = mainPage.MainPartners!.Id;
            entity.LanguageId = languageId;
            mainPartnersDto = _mapper.Map<MainPartnersLocalizationDto>(await UpsertLocalizationAsync(entity, _mainPartnersLocalizationService));
        }

        MainDonationsLocalizationDto? mainDonationsDto = null;
        if (dto.MainDonations is not null)
        {
            var entity = _mapper.Map<MainDonationsLocalization>(dto.MainDonations);
            entity.EntityId = mainPage.MainDonations!.Id;
            entity.LanguageId = languageId;
            mainDonationsDto = _mapper.Map<MainDonationsLocalizationDto>(await UpsertLocalizationAsync(entity, _mainDonationsLocalizationService));
        }

        return (mainAboutUsDto, mainPartnersDto, mainDonationsDto);
    }

    private async Task<TLocalization> UpsertLocalizationAsync<TEntity, TLocalization>(
        TLocalization entity,
        ILocalizationService<TEntity, TLocalization> service)
        where TEntity : class, ITranslatedEntity<TLocalization>, IEntity
        where TLocalization : LocalizationBase<TEntity>
    {
        var existing = await _repositoryWrapper.GetRepository<TLocalization>()
            .GetFirstOrDefaultAsync(new QueryOptions<TLocalization>
            {
                Filter = l => l.EntityId == entity.EntityId && l.LanguageId == entity.LanguageId,
                AsNoTracking = true
            });

        return existing is null
            ? await service.CreateEntityLocalizationAsync(entity)
            : await service.UpdateEntityLocalizationAsync(entity);
    }
}
