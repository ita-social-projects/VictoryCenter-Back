using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Helpers;

public static class ProgramSectionLocalizationProjector
{
    public static List<HippotherapyProgramSectionLocalizationDto> Project(
        IEnumerable<HippotherapyProgramSection> sections,
        long languageId,
        LocalizationInfoDto languageInfo,
        IMapper mapper)
    {
        return sections
            .Select(section => new HippotherapyProgramSectionLocalizationDto
            {
                EntityId = section.Id,
                Contents = section.Contents
                    .Select(content => ProjectContent(content, languageId, languageInfo, mapper))
                    .ToList(),
            })
            .ToList();
    }

    private static HippotherapyProgramSectionContentLocalizationDto ProjectContent(
        ProgramSectionContent content,
        long languageId,
        LocalizationInfoDto languageInfo,
        IMapper mapper)
    {
        var localization = content.Localizations
            .FirstOrDefault(l => l.LanguageId == languageId);

        if (localization is not null)
        {
            return mapper.Map<HippotherapyProgramSectionContentLocalizationDto>(localization);
        }

        return new HippotherapyProgramSectionContentLocalizationDto
        {
            EntityId = content.Id,
            LocalizationInfoDto = languageInfo,
            TranslationStatus = TranslationStatus.Outdated,
        };
    }
}
