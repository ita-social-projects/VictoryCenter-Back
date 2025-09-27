using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Queries.Admin.Donate.UahBankDetails.GetAll;
public class GetAllUahBankDetailsHandler : IRequestHandler<GetAllUahBankDetailsQuery, Result<List<UahBankDetailsDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetAllUahBankDetailsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<UahBankDetailsDto>>> Handle(GetAllUahBankDetailsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Entities.UahBankDetails> uahBankDetails = await _repositoryWrapper.UahBankDetailsRepository.GetAllAsync();
        var mapped = _mapper.Map<IEnumerable<UahBankDetailsDto>>(uahBankDetails).ToList();

        return Result.Ok(mapped);
    }
}
