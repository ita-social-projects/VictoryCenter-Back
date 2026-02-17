using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
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

    public CreateHippotherapyProgramLocalizationHandler(
        IMapper mapper,
        IValidator<CreateHippotherapyProgramLocalizationCommand> validator,
        ILocalizationService<HippotherapyProgramEntity, HippotherapyProgramLocalization> programLocalizationService,
        IRepositoryWrapper repositoryWrapper,
        ILocalizationService<ProgramSectionContent, ProgramSectionContentLocalization> contentLocalizationService)
    {
        _mapper = mapper;
        _validator = validator;
        _programLocalizationService = programLocalizationService;
        _repositoryWrapper = repositoryWrapper;
        _contentLocalizationService = contentLocalizationService;
    }

    public async Task<Result<HippotherapyProgramLocalizationDto>> Handle(CreateHippotherapyProgramLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var contentTypesById = await GetContentTypesById(request.CreateHippotherapyProgramLocalizationDto.EntityId, cancellationToken);
            ValidateSections(request.CreateHippotherapyProgramLocalizationDto.Sections, contentTypesById);

            var hippotherapyProgramLocalizationEntity = _mapper.Map<HippotherapyProgramLocalization>(request.CreateHippotherapyProgramLocalizationDto);
            var createdProgramLocalization = await _programLocalizationService.CreateEntityLocalizationAsync(hippotherapyProgramLocalizationEntity);
            await CreateSectionContentLocalizationsAsync(request.CreateHippotherapyProgramLocalizationDto.Sections);

            var response = _mapper.Map<HippotherapyProgramLocalizationDto>(createdProgramLocalization);

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

    private async Task<Dictionary<long, ContentType>> GetContentTypesById(long programId, CancellationToken cancellationToken)
    {
        var program = await _repositoryWrapper.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<HippotherapyProgramEntity>
            {
                Filter = entity => entity.Id == programId,
                Include = query => query.Include(entity => entity.Sections)
                    .ThenInclude(section => section.Contents),
                AsNoTracking = true
            });

        if (program is null)
        {
            throw new KeyNotFoundException(ErrorMessagesConstants.NotFound(programId, typeof(HippotherapyProgramEntity)));
        }

        return program.Sections
            .SelectMany(section => section.Contents)
            .ToDictionary(content => content.Id, content => content.ContentType);
    }

    private static void ValidateSections(
        IReadOnlyCollection<CreateHippotherapyProgramSectionLocalizationDto> sections,
        IReadOnlyDictionary<long, ContentType> contentTypesById)
    {
        if (sections.Count == 0)
        {
            return;
        }

        var failures = new List<ValidationFailure>();

        foreach (var section in sections)
        {
            if (section.Contents is null)
            {
                continue;
            }

            foreach (var content in section.Contents)
            {
                if (!contentTypesById.TryGetValue(content.EntityId, out var contentType))
                {
                    failures.Add(new ValidationFailure(
                        nameof(content.EntityId),
                        ErrorMessagesConstants.NotFound(content.EntityId, typeof(ProgramSectionContent))));
                    continue;
                }

                ValidateContentLocalizationByType(content, contentType, failures);
            }
        }

        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }
    }

    private static void ValidateContentLocalizationByType(
        CreateHippotherapyProgramSectionContentLocalizationDto content,
        ContentType contentType,
        List<ValidationFailure> failures)
    {
        var hasTitle = HasValue(content.Title);
        var hasDescription = HasValue(content.Description);
        var hasAuthor = HasValue(content.Author);
        var hasQuestion = HasValue(content.Question);
        var hasAnswer = HasValue(content.Answer);

        switch (contentType)
        {
            case ContentType.Title:
                RequireField(failures, nameof(content.Title), hasTitle);
                ForbidField(failures, nameof(content.Description), hasDescription, contentType);
                ForbidField(failures, nameof(content.Author), hasAuthor, contentType);
                ForbidField(failures, nameof(content.Question), hasQuestion, contentType);
                ForbidField(failures, nameof(content.Answer), hasAnswer, contentType);
                break;
            case ContentType.Description:
                RequireField(failures, nameof(content.Description), hasDescription);
                ForbidField(failures, nameof(content.Title), hasTitle, contentType);
                ForbidField(failures, nameof(content.Author), hasAuthor, contentType);
                ForbidField(failures, nameof(content.Question), hasQuestion, contentType);
                ForbidField(failures, nameof(content.Answer), hasAnswer, contentType);
                break;
            case ContentType.Author:
                RequireField(failures, nameof(content.Author), hasAuthor);
                ForbidField(failures, nameof(content.Title), hasTitle, contentType);
                ForbidField(failures, nameof(content.Description), hasDescription, contentType);
                ForbidField(failures, nameof(content.Question), hasQuestion, contentType);
                ForbidField(failures, nameof(content.Answer), hasAnswer, contentType);
                break;
            case ContentType.Question:
                RequireField(failures, nameof(content.Question), hasQuestion);
                ForbidField(failures, nameof(content.Title), hasTitle, contentType);
                ForbidField(failures, nameof(content.Description), hasDescription, contentType);
                ForbidField(failures, nameof(content.Author), hasAuthor, contentType);
                ForbidField(failures, nameof(content.Answer), hasAnswer, contentType);
                break;
            case ContentType.Answer:
                RequireField(failures, nameof(content.Answer), hasAnswer);
                ForbidField(failures, nameof(content.Title), hasTitle, contentType);
                ForbidField(failures, nameof(content.Description), hasDescription, contentType);
                ForbidField(failures, nameof(content.Author), hasAuthor, contentType);
                ForbidField(failures, nameof(content.Question), hasQuestion, contentType);
                break;
            case ContentType.Image:
                ForbidField(failures, nameof(content.Title), hasTitle, contentType);
                ForbidField(failures, nameof(content.Description), hasDescription, contentType);
                ForbidField(failures, nameof(content.Author), hasAuthor, contentType);
                ForbidField(failures, nameof(content.Question), hasQuestion, contentType);
                ForbidField(failures, nameof(content.Answer), hasAnswer, contentType);
                break;
            default:
                failures.Add(new ValidationFailure(
                    nameof(contentType),
                    ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(contentType))));
                break;
        }
    }

    private static void RequireField(List<ValidationFailure> failures, string fieldName, bool hasValue)
    {
        if (!hasValue)
        {
            failures.Add(new ValidationFailure(fieldName, ErrorMessagesConstants.PropertyIsRequired(fieldName)));
        }
    }

    private static void ForbidField(List<ValidationFailure> failures, string fieldName, bool hasValue, ContentType contentType)
    {
        if (hasValue)
        {
            failures.Add(new ValidationFailure(fieldName, $"{fieldName} is not allowed for content type {contentType}"));
        }
    }

    private static bool HasValue(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    private async Task CreateSectionContentLocalizationsAsync(IReadOnlyCollection<CreateHippotherapyProgramSectionLocalizationDto> sections)
    {
        if (sections.Count == 0)
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
