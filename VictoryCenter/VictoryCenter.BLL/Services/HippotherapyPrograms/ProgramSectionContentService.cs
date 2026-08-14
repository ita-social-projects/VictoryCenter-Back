using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Interfaces.HippotherapyPrograms;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Services.HippotherapyPrograms;

public class ProgramSectionContentService : IProgramSectionContentService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public ProgramSectionContentService(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Dictionary<long, ContentType>> GetContentTypesByProgramIdAsync(long programId)
    {
        var program = await _repositoryWrapper.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<HippotherapyProgram>
            {
                Filter = entity => entity.Id == programId,
                Include = query => query.Include(entity => entity.Sections)
                    .ThenInclude(section => section.Contents),
                AsNoTracking = true,
            });

        if (program is null)
        {
            throw new KeyNotFoundException(ErrorMessagesConstants.NotFound(programId, typeof(HippotherapyProgram)));
        }

        return program.Sections
            .SelectMany(section => section.Contents)
            .ToDictionary(content => content.Id, content => content.ContentType);
    }

    public async Task<List<HippotherapyProgramSectionLocalizationDto>> GetProgramSectionsAsync(long programId, long languageId)
    {
        var program = await _repositoryWrapper.HippotherapyProgramsLocalizationsRepository
            .GetFirstOrDefaultAsync(
                new QueryOptions<HippotherapyProgramLocalization>
                {
                    Filter = entity => programId == entity.EntityId
                                       && languageId == entity.LanguageId,
                    Include = query => query.Include(entity => entity.Entity)
                        .ThenInclude(entity => entity.Sections)
                        .ThenInclude(section => section.Contents)
                        .ThenInclude(content => content.Localizations)
                        .ThenInclude(localization => localization.Language)
                        .Include(entity => entity.Language),
                });

        if (program is null)
        {
            throw new KeyNotFoundException(ErrorMessagesConstants.NotFound(programId, typeof(HippotherapyProgram)));
        }

        var languageInfo = _mapper.Map<LocalizationInfoDto>(program.Language);

        return ProgramSectionLocalizationProjector.Project(program.Entity.Sections, languageId, languageInfo, _mapper);
    }
}
