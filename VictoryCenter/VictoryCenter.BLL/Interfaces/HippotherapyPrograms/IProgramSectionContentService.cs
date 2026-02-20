using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Interfaces.HippotherapyPrograms;

public interface IProgramSectionContentService
{
    Task<Dictionary<long, ContentType>> GetContentTypesByProgramIdAsync(long programId);
}
