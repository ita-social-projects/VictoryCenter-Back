using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.HippotherapyLandingPage.Get;

public class GetHippotherapyLandingPageHandler : IRequestHandler<GetHippotherapyLandingPageQuery, Result<HippotherapyLandingPageDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public GetHippotherapyLandingPageHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<HippotherapyLandingPageDto>> Handle(GetHippotherapyLandingPageQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repositoryWrapper.HippotherapyLandingPagesRepository.GetFirstOrDefaultAsync(new QueryOptions<DAL.Entities.HippotherapyLandingPage>
        {
            Include = HippotherapyLandingPageIncludeHelper.IncludeFullGraph,
        });

        if (entity == null)
        {
            return Result.Fail<HippotherapyLandingPageDto>(ErrorMessagesConstants.NotFound());
        }

        return Result.Ok(_mapper.Map<HippotherapyLandingPageDto>(entity));
    }
}
