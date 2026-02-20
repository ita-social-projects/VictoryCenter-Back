using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Interfaces.HippotherapyPrograms;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using HippotherapyProgramEntity = VictoryCenter.DAL.Entities.HippotherapyProgram;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgram.Create;

public class CreateHippotherapyProgramLocalizationHandler : IRequestHandler<CreateHippotherapyProgramLocalizationCommand, Result<HippotherapyProgramLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateHippotherapyProgramLocalizationCommand> _validator;
    private readonly ILocalizationService<HippotherapyProgramEntity, HippotherapyProgramLocalization> _programLocalizationService;
    private readonly ILocalizationService<ProgramSectionContent, ProgramSectionContentLocalization> _contentLocalizationService;
    private readonly IProgramSectionContentService _programSectionContentService;

    public CreateHippotherapyProgramLocalizationHandler(
        IMapper mapper,
        IValidator<CreateHippotherapyProgramLocalizationCommand> validator,
        ILocalizationService<HippotherapyProgramEntity, HippotherapyProgramLocalization> programLocalizationService,
        IRepositoryWrapper repositoryWrapper,
        ILocalizationService<ProgramSectionContent, ProgramSectionContentLocalization> contentLocalizationService,
        IProgramSectionContentService programSectionContentService)
    {
        _mapper = mapper;
        _validator = validator;
        _programLocalizationService = programLocalizationService;
        _repositoryWrapper = repositoryWrapper;
        _contentLocalizationService = contentLocalizationService;
        _programSectionContentService = programSectionContentService;
    }

    public async Task<Result<HippotherapyProgramLocalizationDto>> Handle(CreateHippotherapyProgramLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var contentTypesById = await _programSectionContentService.GetContentTypesByProgramIdAsync(request.CreateHippotherapyProgramLocalizationDto.EntityId);
            ProgramSectionContentLocalizationValidationHelper.ValidateSections(request.CreateHippotherapyProgramLocalizationDto.Sections, contentTypesById);

            HippotherapyProgramLocalization createdProgramLocalization;
            using (var transaction = _repositoryWrapper.BeginTransaction())
            {
                var hippotherapyProgramLocalizationEntity = _mapper.Map<HippotherapyProgramLocalization>(request.CreateHippotherapyProgramLocalizationDto);
                createdProgramLocalization = await _programLocalizationService.CreateEntityLocalizationAsync(hippotherapyProgramLocalizationEntity);
                await CreateSectionContentLocalizationsAsync(request.CreateHippotherapyProgramLocalizationDto.Sections);

                transaction.Complete();
            }

            var sections = await GetProgramSections(createdProgramLocalization.EntityId, createdProgramLocalization.LanguageId);
            var response = _mapper.Map<HippotherapyProgramLocalizationDto>(createdProgramLocalization);
            response = response with { Sections = sections };

            return Result.Ok(response);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<HippotherapyProgramLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<HippotherapyProgramLocalizationDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyProgramLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyProgramLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyProgramLocalizationDto>(ErrorMessagesConstants.
                FailedToCreateEntityInDatabase(typeof(HippotherapyProgramLocalization)));
        }
    }

    private async Task<List<HippotherapyProgramSectionLocalizationDto>> GetProgramSections(long programId, long languageId)
    {
        var program = await _repositoryWrapper.HippotherapyProgramsLocalizationsRepository
            .GetFirstOrDefaultAsync(
                new QueryOptions<HippotherapyProgramLocalization>()
                {
                    Filter = entity => programId == entity.EntityId
                                       && languageId == entity.LanguageId,
                    Include = query => query.Include(entity => entity.Entity)
                        .ThenInclude(entity => entity.Sections)
                        .ThenInclude(section => section.Contents)
                        .ThenInclude(content => content.Localizations)
                        .ThenInclude(localization => localization.Language),
                });

        if (program is null)
        {
            throw new KeyNotFoundException(ErrorMessagesConstants.NotFound(programId, typeof(HippotherapyProgramEntity)));
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

    private async Task CreateSectionContentLocalizationsAsync(IReadOnlyCollection<CreateHippotherapyProgramSectionLocalizationDto> sections)
    {
        if (sections is null || sections.Count == 0)
        {
            return;
        }

        foreach (var section in sections)
        {
            if (section.Contents is null)
            {
                continue;
            }

            foreach (var content in section.Contents)
            {
                var mappedContent = _mapper.Map<ProgramSectionContentLocalization>(content);
                await _contentLocalizationService.CreateEntityLocalizationAsync(mappedContent);
            }
        }
    }
}
