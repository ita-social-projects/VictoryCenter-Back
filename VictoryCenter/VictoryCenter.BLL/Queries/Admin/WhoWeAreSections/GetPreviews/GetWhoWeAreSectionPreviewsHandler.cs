using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.Common;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Queries.Admin.WhoWeAreSections.GetPreviews;

public class GetWhoWeAreSectionPreviewsHandler : IRequestHandler<GetWhoWeAreSectionPreviewsQuery, Result<List<WhoWeAreSectionInfoDto>>>
{
    private readonly IRepositoryWrapper _repository;

    public GetWhoWeAreSectionPreviewsHandler(IRepositoryWrapper repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<WhoWeAreSectionInfoDto>>> Handle(GetWhoWeAreSectionPreviewsQuery request, CancellationToken cancellationToken)
    {
        var rows = await _repository.WhoWeAreSectionsRepository.GetPreviewRowsAsync(cancellationToken);
        var result = new List<WhoWeAreSectionInfoDto>();
        Dictionary<long, WhoWeAreSectionInfoDto> sections = [];
        foreach (var row in rows)
        {
            if (!sections.TryGetValue(row.Id, out var section))
            {
                section = new WhoWeAreSectionInfoDto { Id = row.Id, SectionType = row.SectionType.ToString(), Title = row.Title };
                sections.Add(row.Id, section);
                result.Add(section);
            }

            if (row.LanguageId is { } languageId && row.TranslationStatus is { } translationStatus)
            {
                section.TranslationStatuses.Add(new TranslationStatusInfoDto { LanguageId = languageId, TranslationStatus = translationStatus });
            }
        }

        return Result.Ok(result);
    }
}
