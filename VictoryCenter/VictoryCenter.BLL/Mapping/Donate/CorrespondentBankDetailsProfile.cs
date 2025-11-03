using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.Donate;
public class CorrespondentBankDetailsProfile : Profile
{
    public CorrespondentBankDetailsProfile()
    {
        CreateMap<CorrespondentBankDetails, CorrespondentBankDetailsDto>();
        CreateMap<CorrespondentBankDetails, DTOs.Public.Donate.CorrespondentBankDetails.CorrespondentBankDetailsDto>();
        CreateMap<CreateCorrespondentBankDetailsDto, CorrespondentBankDetails>();
        CreateMap<UpdateCorrespondentBankDetailsDto, CorrespondentBankDetails>();
    }
}
