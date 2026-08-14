using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Update;

namespace VictoryCenter.BLL.Interfaces.HippotherapyPrograms;

public interface IProgramSectionContentLocalizationTracker
{
    Task TrackAsync(
        IEnumerable<UpdateHippotherapyProgramSectionContentLocalizationDto> contentDtos,
        long languageId);
}
