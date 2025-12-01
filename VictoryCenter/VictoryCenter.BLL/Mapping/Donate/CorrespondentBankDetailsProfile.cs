using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
using VictoryCenter.BLL.DTOs.Public.Donate.CorrespondentBankDetails;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.Donate;

public class CorrespondentBankDetailsProfile : Profile
{
    public CorrespondentBankDetailsProfile()
    {
        CreateMap<CorrespondentBankDetails, CorrespondentBankDetailsDto>();
        CreateMap<CorrespondentBankDetails, PublishedCorrespondentBankDetailsDto>();
        CreateMap<CreateCorrespondentBankDetailsDto, CorrespondentBankDetails>();
        CreateMap<UpdateCorrespondentBankDetailsDto, CorrespondentBankDetails>();
    }
}
