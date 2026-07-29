using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.DTOs.Public.Partners;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Public.Partners.GetPage;

public class GetPartnersPageHandler : IRequestHandler<GetPartnersPageQuery, Result<PartnersPageDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetPartnersPageHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<PartnersPageDto>> Handle(GetPartnersPageQuery request, CancellationToken cancellationToken)
    {
        var partnerSections = await _repositoryWrapper.PartnerSectionsRepository
            .GetAllAsync(new QueryOptions<PartnerSection>
            {
                Include = q => q
                    .Include(s => s.Partners.OrderBy(p => p.Priority))
                    .ThenInclude(p => p.Image!)
                    .Include(s => s.Partners.OrderBy(p => p.Priority))
                    .ThenInclude(p => p.Localizations)
                        .ThenInclude(l => l.Language)
                    .Include(s => s.Localizations)
                        .ThenInclude(l => l.Language),
                OrderByASC = s => s.Priority,
                AsNoTracking = true
            });

        var banner = await _repositoryWrapper.PartnersPageBannersRepository
            .GetFirstOrDefaultAsync(new()
            {
                Include = q => q.Include(b => b.Image!)
                                .Include(b => b.Localizations)
                                    .ThenInclude(l => l.Language),
                AsNoTracking = true
            });

        PartnersPageBannerDto bannerDto;

        if (banner == null)
        {
            bannerDto = new PartnersPageBannerDto
            {
                Title = string.Empty,
                Description = string.Empty,
                Image = null
            };
        }
        else
        {
            bannerDto = _mapper.Map<PartnersPageBannerDto>(banner);
        }

        var partnersPageDto = new PartnersPageDto
        {
            Banner = bannerDto,
            Sections = _mapper.Map<IEnumerable<PartnersSectionDto>>(partnerSections)
        };

        return Result.Ok(partnersPageDto);
    }
}
