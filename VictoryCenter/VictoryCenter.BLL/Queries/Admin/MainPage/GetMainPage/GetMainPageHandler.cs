using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.BLL.Queries.Admin.MainPage.GetMainPage;

public class GetMainPageHandler : IRequestHandler<GetMainPageQuery, Result<MainPageDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public GetMainPageHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<MainPageDto>> Handle(GetMainPageQuery request, CancellationToken cancellationToken)
    {
        var mainPageEntity = await _repositoryWrapper.MainPageRepository
            .GetFirstOrDefaultAsync(new QueryOptions<MainPageEntity>
            {
                Include = q => q
                    .Include(e => e.Image)
                    .Include(e => e.MainAboutUs)
                    .Include(e => e.MainPartners)
                    .Include(e => e.ImpactStatistics)
                    .ThenInclude(s => s.Image)
                    .Include(e => e.ImpactStatistics)
                    .ThenInclude(s => s.Metrics),
                AsNoTracking = true
            });

        if (mainPageEntity is null)
        {
            return Result.Fail<MainPageDto>(
                ErrorMessagesConstants.NotFound(null, typeof(MainPageEntity)));
        }

        var resultDto = _mapper.Map<MainPageEntity, MainPageDto>(mainPageEntity);

        return Result.Ok(resultDto);
    }
}
