using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.History;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.History.GetByLanguageId;

public class GetHistoryLocalizationsByLanguageIdHandler : IRequestHandler<GetHistoryLocalizationsByLanguageIdQuery, Result<List<HistorySectionLocalizationDto>>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public GetHistoryLocalizationsByLanguageIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<List<HistorySectionLocalizationDto>>> Handle(GetHistoryLocalizationsByLanguageIdQuery request, CancellationToken cancellationToken)
    {
        var sections = await _repositoryWrapper.HistorySectionsRepository
            .GetAllAsync(new QueryOptions<HistorySection>
            {
                Filter = x => x.Contents.Any(c => c.Localizations.Any(l => l.LanguageId == request.LanguageId)),
                Include = x => x.Include(s => s.Contents)
                    .ThenInclude(c => c.Localizations.Where(l => l.LanguageId == request.LanguageId))
                    .ThenInclude(l => l.Language),
                AsNoTracking = true
            });

        var result = sections
            .Select(section => new HistorySectionLocalizationDto
            {
                EntityId = section.Id,
                Contents = _mapper.Map<List<HistorySectionContentLocalizationDto>>(
                    section.Contents.SelectMany(c => c.Localizations))
            })
            .ToList();

        return Result.Ok(result);
    }
}
