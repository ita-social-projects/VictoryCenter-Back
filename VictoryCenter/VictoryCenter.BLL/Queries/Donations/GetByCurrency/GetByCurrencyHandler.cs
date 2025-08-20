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

namespace VictoryCenter.BLL.Queries.Donations.GetByCurrency;

public class GetByCurrencyHandler : IRequestHandler<GetByCurrencyQuery, Result<DonationsByCurrencyDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public GetByCurrencyHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<DonationsByCurrencyDto>> Handle(GetByCurrencyQuery request, CancellationToken cancellationToken)
    {
        var currency = request.Currency;

        BankDetailsDto bankDetailsDto;
        SupportMethodDto otherMethodDto;

        if (currency == Currency.Uah)
        {
            var latestUahBank = (await _repositoryWrapper.UahBankDetailsRepository.GetAllAsync(new QueryOptions<UahBankDetails>
            {
                Include = q => q.Include(x => x.AdditionalFields),
                OrderByDESC = x => x.CreatedAt,
                Limit = 1
            })).FirstOrDefault();

            var latestUahSupport = (await _repositoryWrapper.SupportMethodRepository.GetAllAsync(new QueryOptions<SupportMethod>
            {
                Include = q => q.Include(x => x.AdditionalFields),
                Filter = x => x.Currency == Currency.Uah,
                OrderByDESC = x => x.CreatedAt,
                Limit = 1
            })).FirstOrDefault();

            bankDetailsDto = latestUahBank != null ? _mapper.Map<BankDetailsDto>(latestUahBank) : new BankDetailsDto();
            otherMethodDto = latestUahSupport != null ? _mapper.Map<SupportMethodDto>(latestUahSupport) : new SupportMethodDto();
        }
        else
        {
            var latestForeignBank = (await _repositoryWrapper.ForeignBankDetailsRepository.GetAllAsync(new QueryOptions<ForeignBankDetails>
            {
                Include = q => q
                    .Include(x => x.AdditionalFields)
                    .Include(x => x.CorrespondentBanks),
                Filter = x => x.Currency == currency,
                OrderByDESC = x => x.CreatedAt,
                Limit = 1
            })).FirstOrDefault();

            var latestSupport = (await _repositoryWrapper.SupportMethodRepository.GetAllAsync(new QueryOptions<SupportMethod>
            {
                Include = q => q.Include(x => x.AdditionalFields),
                Filter = x => x.Currency == currency,
                OrderByDESC = x => x.CreatedAt,
                Limit = 1
            })).FirstOrDefault();

            bankDetailsDto = latestForeignBank != null ? _mapper.Map<BankDetailsDto>(latestForeignBank) : new BankDetailsDto();
            otherMethodDto = latestSupport != null ? _mapper.Map<SupportMethodDto>(latestSupport) : new SupportMethodDto();
        }

        var dto = new DonationsByCurrencyDto
        {
            BankDetails = bankDetailsDto,
            OtherMethod = otherMethodDto
        };

        return Result.Ok(dto);
    }
}
