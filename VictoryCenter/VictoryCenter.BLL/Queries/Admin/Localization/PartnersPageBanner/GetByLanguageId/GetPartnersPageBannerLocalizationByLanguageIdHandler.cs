using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnersPageBanner;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.PartnersPageBanner.GetByLanguageId;

public class GetPartnersPageBannerLocalizationByLanguageIdHandler
    : IRequestHandler<GetPartnersPageBannerLocalizationByLanguageIdQuery, Result<PartnersPageBannerLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetPartnersPageBannerLocalizationByLanguageIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<PartnersPageBannerLocalizationDto>> Handle(
        GetPartnersPageBannerLocalizationByLanguageIdQuery request,
        CancellationToken cancellationToken)
    {
        var localization = await _repositoryWrapper.PartnersPageBannerLocalizationsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<PartnersPageBannerLocalization>
            {
                Filter = l => l.EntityId == request.EntityId && l.LanguageId == request.LanguageId,
                Include = query => query.Include(l => l.Language),
                AsNoTracking = true
            });

        if (localization is null)
        {
            return Result.Fail<PartnersPageBannerLocalizationDto>(
                ErrorMessagesConstants.NotFound((request.EntityId, request.LanguageId), typeof(PartnersPageBannerLocalization)));
        }

        return Result.Ok(_mapper.Map<PartnersPageBannerLocalizationDto>(localization));
    }
}
