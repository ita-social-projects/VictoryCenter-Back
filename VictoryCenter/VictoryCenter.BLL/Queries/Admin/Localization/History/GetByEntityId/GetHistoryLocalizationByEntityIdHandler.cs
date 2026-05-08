using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.History;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.History.GetByEntityId;

public class GetHistoryLocalizationByEntityIdHandler : IRequestHandler<GetHistoryLocalizationByEntityIdQuery, Result<IEnumerable<HistorySectionLocalizationDto>>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    public GetHistoryLocalizationByEntityIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<HistorySectionLocalizationDto>>> Handle(GetHistoryLocalizationByEntityIdQuery request, CancellationToken cancellationToken)
    {
        var section = await _repositoryWrapper.HistorySectionsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<HistorySection>
            {
                Filter = x => x.Id == request.EntityId,
                Include = x => x.Include(s => s.Contents)
                    .ThenInclude(c => c.Localizations)
                    .ThenInclude(l => l.Language)
            });

        if (section is null)
        {
            return Result.Fail(ErrorMessagesConstants.NotFound(request.EntityId, typeof(HistorySection)));
        }

        var result = section.Contents
            .SelectMany(c => c.Localizations)
            .GroupBy(l => l.LanguageId)
            .Select(group => new HistorySectionLocalizationDto
            {
                EntityId = section.Id,
                Contents = _mapper.Map<List<HistorySectionContentLocalizationDto>>(group)
            })
            .ToList();

        return Result.Ok(result.AsEnumerable());
    }
}
