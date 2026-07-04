using AutoMapper;
using FluentResults;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Interfaces.MainPage;
using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.BLL.Services.MainPage;

public class MainPageBlocksLocalizationUpdater : IMainPageBlocksLocalizationUpdater
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILocalizationService<MainAboutUs, MainAboutUsLocalization> _mainAboutUsService;
    private readonly ILocalizationService<MainPartners, MainPartnersLocalization> _mainPartnersService;
    private readonly ILocalizationService<MainDonations, MainDonationsLocalization> _mainDonationsService;

    public MainPageBlocksLocalizationUpdater(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILocalizationService<MainAboutUs, MainAboutUsLocalization> mainAboutUsService,
        ILocalizationService<MainPartners, MainPartnersLocalization> mainPartnersService,
        ILocalizationService<MainDonations, MainDonationsLocalization> mainDonationsService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _mainAboutUsService = mainAboutUsService;
        _mainPartnersService = mainPartnersService;
        _mainDonationsService = mainDonationsService;
    }

    public async Task<Result<(MainAboutUsLocalizationDto? MainAboutUs, MainPartnersLocalizationDto? MainPartners, MainDonationsLocalizationDto? MainDonations)>> UpdateBlocksAsync(
        UpdateMainPageLocalizationDto dto,
        MainPageEntity mainPage,
        long languageId)
    {
        if (!ValidateBlocksExist(dto, mainPage))
        {
            return Result.Fail(ErrorMessagesConstants.NotFound());
        }

        MainAboutUsLocalizationDto? mainAboutUsDto = null;
        if (dto.MainAboutUs is not null)
        {
            var entity = _mapper.Map<MainAboutUsLocalization>(dto.MainAboutUs);
            entity.EntityId = mainPage.MainAboutUs!.Id;
            entity.LanguageId = languageId;
            mainAboutUsDto = _mapper.Map<MainAboutUsLocalizationDto>(await UpsertLocalizationAsync(entity, _mainAboutUsService));
        }

        MainPartnersLocalizationDto? mainPartnersDto = null;
        if (dto.MainPartners is not null)
        {
            var entity = _mapper.Map<MainPartnersLocalization>(dto.MainPartners);
            entity.EntityId = mainPage.MainPartners!.Id;
            entity.LanguageId = languageId;
            mainPartnersDto = _mapper.Map<MainPartnersLocalizationDto>(await UpsertLocalizationAsync(entity, _mainPartnersService));
        }

        MainDonationsLocalizationDto? mainDonationsDto = null;
        if (dto.MainDonations is not null)
        {
            var entity = _mapper.Map<MainDonationsLocalization>(dto.MainDonations);
            entity.EntityId = mainPage.MainDonations!.Id;
            entity.LanguageId = languageId;
            mainDonationsDto = _mapper.Map<MainDonationsLocalizationDto>(await UpsertLocalizationAsync(entity, _mainDonationsService));
        }

        return Result.Ok((mainAboutUsDto, mainPartnersDto, mainDonationsDto));
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
