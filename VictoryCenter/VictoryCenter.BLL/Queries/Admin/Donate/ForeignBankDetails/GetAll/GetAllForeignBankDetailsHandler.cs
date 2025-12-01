using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Queries.Admin.Donate.ForeignBankDetails.GetAll;

public class GetAllForeignBankDetailsHandler : IRequestHandler<GetAllForeignBankDetailsQuery, Result<List<ForeignBankDetailsDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetAllForeignBankDetailsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<ForeignBankDetailsDto>>> Handle(GetAllForeignBankDetailsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Entities.ForeignBankDetails> foreignBankDetails = await _repositoryWrapper.ForeignBankDetailsRepository.GetAllAsync(new QueryOptions<Entities.ForeignBankDetails>
        {
            Filter = so => so.Currency == request.Currency,
            Include = entity => entity
                .Include(e => e.CorrespondentBanks)
        });
        var mapped = _mapper.Map<List<ForeignBankDetailsDto>>(foreignBankDetails);

        return Result.Ok(mapped);
    }
}
