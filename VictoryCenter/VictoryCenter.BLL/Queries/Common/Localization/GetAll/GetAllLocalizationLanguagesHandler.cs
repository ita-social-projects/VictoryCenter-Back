using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Queries.Common.Localization.GetAll;

public class GetAllLocalizationLanguagesHandler : IRequestHandler<GetAllLocalizationLanguagesQuery, Result<IEnumerable<LocalizationLanguageDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetAllLocalizationLanguagesHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<IEnumerable<LocalizationLanguageDto>>> Handle(
        GetAllLocalizationLanguagesQuery request,
        CancellationToken cancellationToken)
    {
        var entities = await _repositoryWrapper.LocalizationLanguagesRepository.GetAllAsync();
        return Result.Ok(_mapper.Map<IEnumerable<LocalizationLanguageDto>>(entities));
    }
}
