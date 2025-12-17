using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.FaqQuestions;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.FaqQuestions.GetByLanguageId;

public class GetByLanguageIdHandler : IRequestHandler<GetByLanguageIdQuery, Result<IEnumerable<FaqQuestionLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;

    public GetByLanguageIdHandler(IMapper mapper, IRepositoryWrapper repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Result<IEnumerable<FaqQuestionLocalizationDto>>> Handle(GetByLanguageIdQuery request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<FaqQuestionLocalization>
        {
            Filter = l => l.LanguageId == request.Id,
            Include = l => l.Include(loc => loc.Language),
        };
        IEnumerable<FaqQuestionLocalization> localizations = await _repository.FaqQuestionLocalizationsRepository.GetAllAsync(queryOptions);
        List<FaqQuestionLocalizationDto>? localizationsDto = _mapper.Map<List<FaqQuestionLocalizationDto>>(localizations);

        return Result.Ok(localizationsDto.AsEnumerable());
    }
}
