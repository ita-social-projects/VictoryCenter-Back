using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Queries.Admin.Donate.SupportOptions.GetAll;
public class GetAllSupportOptionsHandler : IRequestHandler<GetAllSupportOptionsQuery, Result<List<SupportOptionsDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetAllSupportOptionsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<SupportOptionsDto>>> Handle(GetAllSupportOptionsQuery request, CancellationToken cancellationToken)
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
