using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.BLL.Queries.Admin.Localization.MainPage.GetByLanguageId;

public class GetMainPageLocalizationByLanguageIdHandler
    : IRequestHandler<GetMainPageLocalizationByLanguageIdQuery, Result<MainPageLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetMainPageLocalizationByLanguageIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<MainPageLocalizationDto>> Handle(
        GetMainPageLocalizationByLanguageIdQuery request,
        CancellationToken cancellationToken)
    {
        var mainPage = await _repositoryWrapper.MainPageRepository
            .GetFirstOrDefaultAsync(new QueryOptions<MainPageEntity>
            {
                Filter = e => e.Id == request.EntityId,
                Include = query => query
                    .Include(e => e.MainAboutUs)
                    .Include(e => e.MainPartners)
                    .Include(e => e.MainDonations),
                AsNoTracking = true
            });

        if (mainPage is null)
        {
            return Result.Fail<MainPageLocalizationDto>(
                ErrorMessagesConstants.NotFound(request.EntityId, typeof(MainPageEntity)));
        }

        var mainPageLocalization = await _repositoryWrapper.MainPageLocalizationsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<MainPageLocalization>
            {
                Filter = l => l.EntityId == request.EntityId && l.LanguageId == request.LanguageId,
                Include = query => query.Include(l => l.Language),
                AsNoTracking = true
            });

        if (mainPageLocalization is null)
        {
            return Result.Fail<MainPageLocalizationDto>(
                ErrorMessagesConstants.NotFound(
                    (request.EntityId, request.LanguageId),
                    typeof(MainPageLocalization)));
        }

        var response = _mapper.Map<MainPageLocalizationDto>(mainPageLocalization) with
        {
            MainAboutUs = await GetMainAboutUsLocalizationAsync(mainPage, request.LanguageId),
            MainPartners = await GetMainPartnersLocalizationAsync(mainPage, request.LanguageId),
            MainDonations = await GetMainDonationsLocalizationAsync(mainPage, request.LanguageId)
        };

        return Result.Ok(response);
    }

    private async Task<MainAboutUsLocalizationDto?> GetMainAboutUsLocalizationAsync(
        MainPageEntity mainPage,
        long languageId)
    {
        if (mainPage.MainAboutUs is null)
        {
            return null;
        }

        var localization = await _repositoryWrapper.MainAboutUsLocalizationsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<MainAboutUsLocalization>
            {
                Filter = l => l.EntityId == mainPage.MainAboutUs.Id && l.LanguageId == languageId,
                Include = query => query.Include(l => l.Language),
                AsNoTracking = true
            });

        return _mapper.Map<MainAboutUsLocalizationDto?>(localization);
    }

    private async Task<MainPartnersLocalizationDto?> GetMainPartnersLocalizationAsync(
        MainPageEntity mainPage,
        long languageId)
    {
        if (mainPage.MainPartners is null)
        {
            return null;
        }

        var localization = await _repositoryWrapper.MainPartnersLocalizationsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<MainPartnersLocalization>
            {
                Filter = l => l.EntityId == mainPage.MainPartners.Id && l.LanguageId == languageId,
                Include = query => query.Include(l => l.Language),
                AsNoTracking = true
            });

        return _mapper.Map<MainPartnersLocalizationDto?>(localization);
    }

    private async Task<MainDonationsLocalizationDto?> GetMainDonationsLocalizationAsync(
        MainPageEntity mainPage,
        long languageId)
    {
        if (mainPage.MainDonations is null)
        {
            return null;
        }

        var localization = await _repositoryWrapper.MainDonationsLocalizationsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<MainDonationsLocalization>
            {
                Filter = l => l.EntityId == mainPage.MainDonations.Id && l.LanguageId == languageId,
                Include = query => query.Include(l => l.Language),
                AsNoTracking = true
            });

        return _mapper.Map<MainDonationsLocalizationDto?>(localization);
    }
}
