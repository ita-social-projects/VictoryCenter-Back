using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyPrograms.GetByEntityId;

public class GetHippotherapyProgramLocalizationByEntityIdHandler : IRequestHandler<GetHippotherapyProgramLocalizationByEntityIdQuery, Result<IEnumerable<HippotherapyProgramLocalizationDto>>>
{
    private readonly IRepositoryWrapper _wrapper;
    private readonly IMapper _mapper;

    public GetHippotherapyProgramLocalizationByEntityIdHandler(IRepositoryWrapper wrapper, IMapper mapper)
    {
        _wrapper = wrapper;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<HippotherapyProgramLocalizationDto>>> Handle(GetHippotherapyProgramLocalizationByEntityIdQuery request, CancellationToken cancellationToken)
    {
        var getProgramLocalizations = new QueryOptions<HippotherapyProgramLocalization>()
        {
            Filter = entity => entity.EntityId == request.Id,
            AsNoTracking = true,
            Include = query => query.Include(entity => entity.Entity)
                .ThenInclude(entity => entity.Sections)
                .ThenInclude(section => section.Contents)
                .ThenInclude(content => content.Localizations)
                .ThenInclude(localization => localization.Language)
                .Include(program => program.Language)
        };
        var programs = (await _wrapper.HippotherapyProgramsLocalizationsRepository
            .GetAllAsync(getProgramLocalizations)).ToList();
        var programLocalizations = _mapper.Map<IEnumerable<HippotherapyProgramLocalizationDto>>(programs);
        var list = new Dictionary<long, List<HippotherapyProgramSectionLocalizationDto>>();
        foreach (var program in programs)
        {
            var sectionDto = await GetProgramSections(program, program.LanguageId);
            list.Add(program.LanguageId, sectionDto);
        }

        programLocalizations = programLocalizations.Select(program =>
        {
            if (list.TryGetValue(program.LocalizationInfoDto.Id, out var sections))
            {
                return program with { Sections = sections };
            }

            return program;
        }).ToList();

        return Result.Ok(programLocalizations);
    }

    private async Task<List<HippotherapyProgramSectionLocalizationDto>> GetProgramSections(HippotherapyProgramLocalization program, long languageId)
    {
        if(program is null)
        {
            throw new KeyNotFoundException(ErrorMessagesConstants.NotFound(nameof(HippotherapyProgramLocalization), typeof(HippotherapyProgramLocalization)));
        }

        var sectionLocalizations = program.Entity
            .Sections
            .Select(section => new HippotherapyProgramSectionLocalizationDto
            {
                EntityId = section.Id,
                Contents = section.Contents
                    .SelectMany(content =>
                        content.Localizations
                            .Where(localization => localization.LanguageId == languageId)
                            .Select(localization =>
                                _mapper.Map<HippotherapyProgramSectionContentLocalizationDto>(localization)))
                    .ToList(),
            })
            .ToList();

        return sectionLocalizations;
    }
}
