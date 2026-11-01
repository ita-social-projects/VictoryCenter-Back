using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.BLL.Services.HippotherapyLandingPages;

public class HippotherapyLandingPageResponseFactory : IHippotherapyLandingPageResponseFactory
{
    private readonly IMapper _mapper;
    private readonly IHippotherapyLandingPageLocalizationBuilder _builder;

    public HippotherapyLandingPageResponseFactory(
        IMapper mapper, IHippotherapyLandingPageLocalizationBuilder builder)
    {
        _mapper = mapper;
        _builder = builder;
    }

    public HippotherapyLandingPageDto Create(VictoryCenter.DAL.Entities.HippotherapyLandingPage page)
    {
        var dto = _mapper.Map<HippotherapyLandingPageDto>(page);
        var source = _builder.Build(page);

        return dto with
        {
            Localizations = HippotherapyLandingPageLocalizationAggregator.BuildLocalizations(source),
            TranslationStatuses = HippotherapyLandingPageLocalizationAggregator.BuildStatuses(source),
        };
    }
}
