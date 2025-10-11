using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.DTOs.Public.Partners;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Public.Partners.GetPage;

public class GetPartnersPageHandler : IRequestHandler<GetPartnersPageQuery, Result<PartnersPageDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetPartnersPageHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IBlobService blobService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<PartnersPageDto>> Handle(GetPartnersPageQuery request, CancellationToken cancellationToken)
    {
        var sectionsTask = _repositoryWrapper.PartnerSectionsRepository
            .GetAllAsync(new QueryOptions<PartnerSection>
            {
                Include = q => q
                    .Include(s => s.Partners.OrderBy(p => p.Priority))
                    .ThenInclude(p => p.Image!),
                OrderByASC = s => s.Priority,
                AsNoTracking = true
            });

        var bannerTask = _repositoryWrapper.PartnersPageBannersRepository.GetFirstOrDefaultAsync(new()
        {
            Include = q => q.Include(b => b.Image!),
            AsNoTracking = true
        });

        await Task.WhenAll(sectionsTask, bannerTask);

        var partnerSections = await sectionsTask;
        var banner = await bannerTask;

        var partnersPageDto = new PartnersPageDto
        {
            Banner = _mapper.Map<PartnersPageBannerDto>(banner),
            Sections = _mapper.Map<IEnumerable<PartnersSectionDto>>(partnerSections)
        };

        return Result.Ok(partnersPageDto);
    }
}
