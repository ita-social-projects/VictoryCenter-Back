using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyPrograms.GetByLanguageId;

public class GetHippotherapyProgramLocalizationByLanguageIdHandler : IRequestHandler<GetHippotherapyProgramLocalizationByLanguageIdQuery, Result<List<HippotherapyProgramLocalizationDto>>>
{
    private readonly IRepositoryWrapper _wrapper;
    private readonly IMapper _mapper;

    public GetHippotherapyProgramLocalizationByLanguageIdHandler(IRepositoryWrapper wrapper, IMapper mapper)
    {
        _wrapper = wrapper;
        _mapper = mapper;
    }

    public async Task<Result<List<HippotherapyProgramLocalizationDto>>> Handle(GetHippotherapyProgramLocalizationByLanguageIdQuery request, CancellationToken cancellationToken)
    {
        var programLocalizations = (await _wrapper.HippotherapyProgramsLocalizationsRepository
            .GetAllAsync(new QueryOptions<HippotherapyProgramLocalization>()
            {
                Filter = entity => request.Id == entity.LanguageId,
                Include = query => query.Include(entity => entity.Language),
                AsNoTracking = true
            })).ToList();

        var resultPrograms = _mapper.Map<List<HippotherapyProgramLocalizationDto>>(programLocalizations);

        return Result.Ok(resultPrograms);
    }
}
