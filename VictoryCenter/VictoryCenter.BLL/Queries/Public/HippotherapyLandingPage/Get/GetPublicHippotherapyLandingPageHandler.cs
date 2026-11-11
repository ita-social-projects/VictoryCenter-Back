using FluentResults;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.BLL.Errors;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Services.HippotherapyLandingPages;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Public.HippotherapyLandingPage.Get;

public class GetPublicHippotherapyLandingPageHandler : IRequestHandler<GetPublicHippotherapyLandingPageQuery, Result<HippotherapyLandingPageDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IHippotherapyLandingPageResponseFactory _responseFactory;

    public GetPublicHippotherapyLandingPageHandler(
        IRepositoryWrapper repositoryWrapper,
        IHippotherapyLandingPageResponseFactory responseFactory)
    {
        _repositoryWrapper = repositoryWrapper;
        _responseFactory = responseFactory;
    }

    public async Task<Result<HippotherapyLandingPageDto>> Handle(
        GetPublicHippotherapyLandingPageQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _repositoryWrapper.HippotherapyLandingPagesRepository.GetFirstOrDefaultAsync(
            new QueryOptions<DAL.Entities.HippotherapyLandingPage>
            {
                Include = HippotherapyLandingPageIncludeHelper.IncludeFullGraph,
                AsSplitQuery = true,
            });

        if (entity == null || !entity.HasAllSections())
        {
            return Result.Fail<HippotherapyLandingPageDto>(new NotFoundError(ErrorMessagesConstants.NotFound()));
        }

        var dto = _responseFactory.Create(entity);
        return Result.Ok(dto with { TranslationStatuses = [] });
    }
}
