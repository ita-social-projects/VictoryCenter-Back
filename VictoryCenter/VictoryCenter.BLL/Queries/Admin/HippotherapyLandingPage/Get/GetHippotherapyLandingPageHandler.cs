using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Services.HippotherapyLandingPages;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.BLL.Errors;

namespace VictoryCenter.BLL.Queries.Admin.HippotherapyLandingPage.Get;

public class GetHippotherapyLandingPageHandler
    : IRequestHandler<GetHippotherapyLandingPageQuery, Result<HippotherapyLandingPageDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IHippotherapyLandingPageResponseFactory _responseFactory;

    public GetHippotherapyLandingPageHandler(
        IRepositoryWrapper repositoryWrapper,
        IHippotherapyLandingPageResponseFactory responseFactory)
    {
        _repositoryWrapper = repositoryWrapper;
        _responseFactory = responseFactory;
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
            return Result.Fail<HippotherapyLandingPageDto>(NotFoundErrors.NotFound());
        }

        return Result.Ok(_responseFactory.Create(entity));
    }
}
