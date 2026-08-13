using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.MainAboutUs;
using VictoryCenter.BLL.DTOs.Admin.MainDonations;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.BLL.DTOs.Admin.MainPartners;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.BLL.Queries.Public.MainPage.GetMainPage;

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
                    .Include(e => e.Localizations).ThenInclude(l => l.Language)
                    .Include(e => e.MainAboutUs).ThenInclude(a => a!.Localizations).ThenInclude(l => l.Language)
                    .Include(e => e.MainPartners).ThenInclude(p => p!.Localizations).ThenInclude(l => l.Language)
                    .Include(e => e.MainDonations).ThenInclude(d => d!.Image)
                    .Include(e => e.MainDonations).ThenInclude(d => d!.Localizations).ThenInclude(l => l.Language)
                    .Include(e => e.ImpactStatistics).ThenInclude(s => s!.Image)
                    .Include(e => e.ImpactStatistics).ThenInclude(s => s!.Localizations)
                    .Include(e => e.ImpactStatistics).ThenInclude(s => s!.Metrics.Where(m => !m.IsHidden).OrderBy(m => m.Priority)).ThenInclude(m => m.Localizations),
                AsNoTracking = true
            });

        if (mainPageEntity is null)
        {
            var emptyMainPageDto = new MainPageDto
            {
                Id = 0,
                Title = string.Empty,
                Description = string.Empty,
                Image = null,
                MainAboutUs = new MainAboutUsDto { Title = string.Empty, Description = string.Empty },
                MainPartners = new MainPartnersDto { Title = string.Empty, Description = string.Empty },
                MainDonations = new MainDonationsDto { Title = string.Empty, Description = string.Empty },
                ImpactStatistics = new ImpactStatisticDto { Title = string.Empty },
                Localizations = []
            };
            return Result.Ok(emptyMainPageDto);
        }

        return Result.Ok(_mapper.Map<MainPageEntity, MainPageDto>(mainPageEntity));
    }
}
