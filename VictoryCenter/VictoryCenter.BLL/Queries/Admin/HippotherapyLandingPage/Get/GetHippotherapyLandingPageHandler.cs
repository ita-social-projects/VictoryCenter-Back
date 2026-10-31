using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Services.HippotherapyLandingPages;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.HippotherapyLandingPage.Get;

public class GetHippotherapyLandingPageHandler
    : IRequestHandler<GetHippotherapyLandingPageQuery, Result<HippotherapyLandingPageDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IHippotherapyLandingPageLocalizationLoader _localizationLoader;

    public GetHippotherapyLandingPageHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IHippotherapyLandingPageLocalizationLoader localizationLoader)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _localizationLoader = localizationLoader;
    }

    public async Task<Result<HippotherapyLandingPageDto>> Handle(
        GetHippotherapyLandingPageQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repositoryWrapper.HippotherapyLandingPagesRepository.GetFirstOrDefaultAsync(
            new QueryOptions<DAL.Entities.HippotherapyLandingPage>
            {
                Include = HippotherapyLandingPageIncludeHelper.IncludeFullGraph,
                AsSplitQuery = true,
            });

        if (entity == null || !HasAllSections(entity))
        {
            return Result.Fail<HippotherapyLandingPageDto>(ErrorMessagesConstants.NotFound());
        }

        var dto = _mapper.Map<HippotherapyLandingPageDto>(entity);
        var source = await _localizationLoader.LoadAsync(entity, cancellationToken);

        return Result.Ok(dto with
        {
            Localizations = HippotherapyLandingPageLocalizationAggregator.BuildLocalizations(source),
            TranslationStatuses = HippotherapyLandingPageLocalizationAggregator.BuildStatuses(source),
        });
    }

    private static bool HasAllSections(DAL.Entities.HippotherapyLandingPage page)
   => page.IntroSection is not null
       && page.DescriptionSection is not null
       && page.QuoteSection is not null
       && page.HippoventionSection is not null
       && page.HippoventionCenterSection is not null
       && page.AdvantagesSection is not null
       && page.AnalysisSection is not null
       && page.ScientificReferencesSection is not null
       && page.AnotherQuoteSection is not null
       && page.ParticipantsSection is not null
       && page.EthicsSection is not null;
}
