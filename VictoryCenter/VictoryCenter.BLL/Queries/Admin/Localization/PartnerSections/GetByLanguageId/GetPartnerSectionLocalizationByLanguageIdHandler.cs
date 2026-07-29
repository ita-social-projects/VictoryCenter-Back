using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.PartnerSections.GetByLanguageId;

public class GetPartnerSectionLocalizationByLanguageIdHandler
    : IRequestHandler<GetPartnerSectionLocalizationByLanguageIdQuery, Result<PartnerSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetPartnerSectionLocalizationByLanguageIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<PartnerSectionLocalizationDto>> Handle(
        GetPartnerSectionLocalizationByLanguageIdQuery request,
        CancellationToken cancellationToken)
    {
        var section = await _repositoryWrapper.PartnerSectionsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<PartnerSection>
            {
                Filter = s => s.Id == request.EntityId,
                Include = q => q.Include(s => s.Partners.OrderBy(p => p.Priority)),
                AsNoTracking = true
            });

        if (section is null)
        {
            return Result.Fail<PartnerSectionLocalizationDto>(
                ErrorMessagesConstants.NotFound(request.EntityId, typeof(PartnerSection)));
        }

        var sectionLocalization = await _repositoryWrapper.PartnerSectionLocalizationsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<PartnerSectionLocalization>
            {
                Filter = l => l.EntityId == request.EntityId && l.LanguageId == request.LanguageId,
                Include = query => query.Include(l => l.Language),
                AsNoTracking = true
            });

        if (sectionLocalization is null)
        {
            return Result.Fail<PartnerSectionLocalizationDto>(
                ErrorMessagesConstants.NotFound((request.EntityId, request.LanguageId), typeof(PartnerSectionLocalization)));
        }

        var partnerIds = section.Partners.Select(p => p.Id).ToList();

        var partnerLocalizations = partnerIds.Count == 0
            ? []
            : await _repositoryWrapper.PartnerLocalizationsRepository
                .GetAllAsync(new QueryOptions<PartnerLocalization>
                {
                    Filter = l => l.LanguageId == request.LanguageId && partnerIds.Contains(l.EntityId),
                    AsNoTracking = true
                });

        var partnerLocalizationsByPartnerId = partnerLocalizations.ToDictionary(l => l.EntityId);

        var partners = section.Partners
            .Select(p => partnerLocalizationsByPartnerId.TryGetValue(p.Id, out var localization)
                ? new PartnerLocalizationItemDto
                {
                    PartnerId = p.Id,
                    Description = localization.Description,
                    TranslationStatus = localization.TranslationStatus
                }
                : new PartnerLocalizationItemDto
                {
                    PartnerId = p.Id,
                    Description = string.Empty,
                    TranslationStatus = null
                })
            .ToList();

        var response = _mapper.Map<PartnerSectionLocalizationDto>(sectionLocalization) with
        {
            Partners = partners
        };

        return Result.Ok(response);
    }
}
