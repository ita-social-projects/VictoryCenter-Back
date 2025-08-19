using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Donations.Common;
using VictoryCenter.BLL.DTOs.Donations.SupportMethod;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Donations.GetAll;

public class GetAllDonationsHandler : IRequestHandler<GetAllDonationsQuery, Result<DonationsGroupedDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public GetAllDonationsHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<DonationsGroupedDto>> Handle(GetAllDonationsQuery request, CancellationToken cancellationToken)
    {
        var latestUahBank = (await _repositoryWrapper.UahBankDetailsRepository.GetAllAsync(
            new QueryOptions<UahBankDetails>
            {
                Include = q => q.Include(x => x.AdditionalFields),
                OrderByDESC = x => x.CreatedAt,
                Limit = 1
            }))
            .FirstOrDefault();

        var latestUsdBank = (await _repositoryWrapper.ForeignBankDetailsRepository.GetAllAsync(
            new QueryOptions<ForeignBankDetails>
            {
                Include = q => q
                    .Include(x => x.AdditionalFields)
                    .Include(x => x.CorrespondentBanks),
                Filter = x => x.Currency == Currency.Usd,
                OrderByDESC = x => x.CreatedAt,
                Limit = 1
            }))
            .FirstOrDefault();

        var latestEurBank = (await _repositoryWrapper.ForeignBankDetailsRepository.GetAllAsync(
            new QueryOptions<ForeignBankDetails>
            {
                Include = q => q
                    .Include(x => x.AdditionalFields)
                    .Include(x => x.CorrespondentBanks),
                Filter = x => x.Currency == Currency.Eur,
                OrderByDESC = x => x.CreatedAt,
                Limit = 1
            }))
            .FirstOrDefault();

        var latestUahSupport = (await _repositoryWrapper.SupportMethodRepository.GetAllAsync(
            new QueryOptions<SupportMethod>
            {
                Include = q => q.Include(x => x.AdditionalFields),
                Filter = x => x.Currency == Currency.Uah,
                OrderByDESC = x => x.CreatedAt,
                Limit = 1
            }))
            .FirstOrDefault();

        var latestUsdSupport = (await _repositoryWrapper.SupportMethodRepository.GetAllAsync(
            new QueryOptions<SupportMethod>
            {
                Include = q => q.Include(x => x.AdditionalFields),
                Filter = x => x.Currency == Currency.Usd,
                OrderByDESC = x => x.CreatedAt,
                Limit = 1
            }))
            .FirstOrDefault();

        var latestEurSupport = (await _repositoryWrapper.SupportMethodRepository.GetAllAsync(
            new QueryOptions<SupportMethod>
            {
                Include = q => q.Include(x => x.AdditionalFields),
                Filter = x => x.Currency == Currency.Eur,
                OrderByDESC = x => x.CreatedAt,
                Limit = 1
            }))
            .FirstOrDefault();

        var dto = new DonationsGroupedDto
        {
            Uah = new CurrencyDonationsDto
            {
                BankDetailsDto = latestUahBank != null ? _mapper.Map<BankDetailsDto>(latestUahBank) : null,
                SupportMethodDto = latestUahSupport != null ? _mapper.Map<SupportMethodDto>(latestUahSupport) : null
            },
            Usd = new CurrencyDonationsDto
            {
                BankDetailsDto = latestUsdBank != null ? _mapper.Map<BankDetailsDto>(latestUsdBank) : null,
                SupportMethodDto = latestUsdSupport != null ? _mapper.Map<SupportMethodDto>(latestUsdSupport) : null
            },
            Eur = new CurrencyDonationsDto
            {
                BankDetailsDto = latestEurBank != null ? _mapper.Map<BankDetailsDto>(latestEurBank) : null,
                SupportMethodDto = latestEurSupport != null ? _mapper.Map<SupportMethodDto>(latestEurSupport) : null
            }
        };

        return Result.Ok(dto);
    }
}
