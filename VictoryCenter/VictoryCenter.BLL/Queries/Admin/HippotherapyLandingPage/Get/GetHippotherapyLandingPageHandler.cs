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
    private readonly IHippotherapyLandingPageResponseFactory _responseFactory;
    private readonly IHippotherapyLandingPageLocalizationBuilder _builder;

    public GetHippotherapyLandingPageHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IHippotherapyLandingPageResponseFactory responseFactory,
        IHippotherapyLandingPageLocalizationBuilder builder)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _responseFactory = responseFactory;
        _builder = builder;
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

        if (entity == null || !entity.HasAllSections())
        {
            return Result.Fail<HippotherapyLandingPageDto>(ErrorMessagesConstants.NotFound());
        }

        var dto = _mapper.Map<HippotherapyLandingPageDto>(entity);
        var source = _builder.Build(entity);

        return Result.Ok(_responseFactory.Create(entity));
    }
}
