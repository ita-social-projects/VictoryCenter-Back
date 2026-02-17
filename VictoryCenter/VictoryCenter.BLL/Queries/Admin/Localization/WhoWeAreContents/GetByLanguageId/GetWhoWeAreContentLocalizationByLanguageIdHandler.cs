using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.WhoWeAreContents.GetByLanguageId;

public class GetWhoWeAreContentLocalizationByLanguageIdHandler : IRequestHandler<GetWhoWeAreContentLocalizationByLanguageIdQuery, Result<List<WhoWeAreContentLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;

    public GetWhoWeAreContentLocalizationByLanguageIdHandler(IMapper mapper, IRepositoryWrapper repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Result<List<WhoWeAreContentLocalizationDto>>> Handle(GetWhoWeAreContentLocalizationByLanguageIdQuery request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<WhoWeAreContentLocalization>
        {
            Filter = l => l.LanguageId == request.Id,
            Include = l => l.Include(loc => loc.Language),
            AsNoTracking = true,
        };
        IEnumerable<WhoWeAreContentLocalization> localizations = await _repository.WhoWeAreContentLocalizationsRepository.GetAllAsync(queryOptions);
        List<WhoWeAreContentLocalizationDto>? localizationsDto = _mapper.Map<List<WhoWeAreContentLocalizationDto>>(localizations);

        return Result.Ok(localizationsDto);
    }
}
