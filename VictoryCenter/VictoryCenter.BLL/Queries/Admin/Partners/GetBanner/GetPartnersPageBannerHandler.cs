using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Queries.Admin.Partners.GetBanner;

public class GetPartnersPageBannerHandler : IRequestHandler<GetPartnersPageBannerQuery, Result<PartnersPageBannerDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public GetPartnersPageBannerHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<PartnersPageBannerDto>> Handle(GetPartnersPageBannerQuery request, CancellationToken cancellationToken)
    {
        var bannerEntity = await _repositoryWrapper.PartnersPageBannersRepository
            .GetFirstOrDefaultAsync(new()
            {
                Include = q => q.Include(b => b.Image!)
            });

        if (bannerEntity == null)
        {
            var emptyBannerDto = new PartnersPageBannerDto
            {
                Title = string.Empty,
                Description = string.Empty,
                Image = null
            };

            return Result.Ok(emptyBannerDto);
        }

        var bannerDto = _mapper.Map<PartnersPageBannerDto>(bannerEntity);
        return Result.Ok(bannerDto);
    }
}
