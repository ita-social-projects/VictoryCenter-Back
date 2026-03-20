using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Interfaces.HippotherapyPrograms;

public interface IProgramSectionContentService
{
    Task<Dictionary<long, ContentType>> GetContentTypesByProgramIdAsync(long programId);

    Task<List<HippotherapyProgramSectionLocalizationDto>> GetProgramSectionsAsync(long programId, long languageId);
}
