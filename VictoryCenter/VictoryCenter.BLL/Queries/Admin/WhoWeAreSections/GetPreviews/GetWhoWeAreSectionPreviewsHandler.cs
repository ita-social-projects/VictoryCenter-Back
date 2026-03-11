using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.Common;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.WhoWeAreSections.GetPreviews;

public class GetWhoWeAreSectionPreviewsHandler : IRequestHandler<GetWhoWeAreSectionPreviewsQuery, Result<List<WhoWeAreSectionInfoDto>>>
{
    private readonly IRepositoryWrapper _repository;
    private readonly IMapper _mapper;

    public GetWhoWeAreSectionPreviewsHandler(IRepositoryWrapper repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<List<WhoWeAreSectionInfoDto>>> Handle(GetWhoWeAreSectionPreviewsQuery request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<WhoWeAreSection>
        {
            Include = s => s
                   .Include(sec => sec.Contents)
                   .ThenInclude(c => c.Localizations)
        };

        var sections = await _repository.WhoWeAreSectionsRepository.GetAllAsync(queryOptions);

        var result = new List<WhoWeAreSectionInfoDto>();

        foreach (var section in sections)
        {
            var mappedSection = _mapper.Map<WhoWeAreSectionInfoDto>(section);
            mappedSection.TranslationStatuses = AggregateLocalizationStatuses(section.Contents);
            result.Add(mappedSection);
        }

        return Result.Ok(result);
    }

    private List<TranslationStatusInfoDto> AggregateLocalizationStatuses(List<WhoWeAreContent> contents)
    {
        var textContents = contents.Where(c => c.ContentType != ContentType.Image).ToList();

        if (textContents.Count == 0)
        {
            return [];
        }

        return textContents
            .SelectMany(c => c.Localizations)
            .GroupBy(l => l.LanguageId)
            .Where(g => g.Count() == textContents.Count)
            .Select(g => new TranslationStatusInfoDto
            {
                LanguageId = g.Key,
                TranslationStatus = g.Any(l => l.TranslationStatus == TranslationStatus.Outdated)
                    ? TranslationStatus.Outdated
                    : TranslationStatus.Relevant
            })
            .ToList();
    }
}
