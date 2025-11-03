using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.Donate.SupportOptions;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Queries.Public.Donate.SupportOptions.GetPublished;
public class GetPublishedSupportOptionsHandler : IRequestHandler<GetPublishedSupportOptionsQuery, Result<List<SupportOptionsDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetPublishedSupportOptionsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<SupportOptionsDto>>> Handle(GetPublishedSupportOptionsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Entities.SupportOptions> supportOptions = await _repositoryWrapper.SupportOptionsRepository.GetAllAsync(
                new QueryOptions<Entities.SupportOptions>
                {
                    Filter = so => so.Currency == request.Currency
                });

        var mapped = _mapper.Map<List<SupportOptionsDto>>(supportOptions);

        return Result.Ok(mapped);
    }
}
