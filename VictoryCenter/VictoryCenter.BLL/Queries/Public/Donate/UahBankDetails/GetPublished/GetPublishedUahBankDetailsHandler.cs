using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.Donate.UahBankDetails;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Queries.Public.Donate.UahBankDetails.GetPublished;
public class GetPublishedUahBankDetailsHandler : IRequestHandler<GetPublishedUahBankDetailsQuery, Result<List<UahBankDetailsDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetPublishedUahBankDetailsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<UahBankDetailsDto>>> Handle(GetPublishedUahBankDetailsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Entities.UahBankDetails> uahBankDetails = await _repositoryWrapper.UahBankDetailsRepository.GetAllAsync(
            new QueryOptions<Entities.UahBankDetails>());
        var mapped = _mapper.Map<List<UahBankDetailsDto>>(uahBankDetails);

        return Result.Ok(mapped);
    }
}
