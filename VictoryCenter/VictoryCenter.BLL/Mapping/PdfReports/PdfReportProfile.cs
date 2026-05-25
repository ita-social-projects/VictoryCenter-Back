using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.PdfReports;

public class PdfReportProfile : Profile
{
    public PdfReportProfile()
    {
        CreateMap<PdfReport, PdfReportDto>()
            .ForMember(dest => dest.LanguageCode, opt => opt.MapFrom(src => src.Language.Code));
    }
}
