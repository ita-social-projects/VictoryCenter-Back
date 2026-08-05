using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Interfaces.SlugService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Update;

public class UpdateHippotherapyProgramHandler : IRequestHandler<UpdateHippotherapyProgramCommand, Result<HippotherapyProgramDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateHippotherapyProgramCommand> _validator;
    private readonly ISlugService _slugService;

    public UpdateHippotherapyProgramHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateHippotherapyProgramCommand> validator,
        ISlugService slugService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _slugService = slugService;
    }

    public async Task<Result<HippotherapyProgramDto>> Handle(
        UpdateHippotherapyProgramCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var program = await _repositoryWrapper.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(
                new QueryOptions<HippotherapyProgram>
                {
                    Filter = p => p.Id == request.Id,
                    AsNoTracking = false,
                    Include = p => p
                        .Include(x => x.Categories)
                        .Include(x => x.Sections)
                        .ThenInclude(s => s.Contents)
                        .ThenInclude(c => c.Localizations)
                        .ThenInclude(c => c.Language)
                        .Include(x => x.Localizations)
                        .ThenInclude(x => x.Language)
                        .Include(x => x.Sections)
                        .ThenInclude(s => s.Contents)
                        .ThenInclude(c => (c as FaqQuestionProgramContent)!.FaqQuestion)
                });

            if (program is null)
            {
                return Result.Fail<HippotherapyProgramDto>(
                    ErrorMessagesConstants.NotFound(request.Id, typeof(HippotherapyProgram)));
            }

            var newCategoriesResult = await CategoryValidationHelper.ValidateAndGetCategoriesAsync(
                _repositoryWrapper.HippotherapyProgramCategoriesRepository,
                request.UpdateProgramDto.CategoryIds);

            if (newCategoriesResult.IsFailed)
            {
                return Result.Fail<HippotherapyProgramDto>(newCategoriesResult.Errors);
            }

            var imagesByIdResult = await ImageValidationHelper.ValidateAndGetSectionImagesAsync(
                _repositoryWrapper, request.UpdateProgramDto.Sections);

            if (imagesByIdResult.IsFailed)
            {
                return Result.Fail<HippotherapyProgramDto>(imagesByIdResult.Errors);
            }

            var oldSlug = program.Slug;
            var nameChanged = request.UpdateProgramDto.Name != program.Name;
            var programFieldsChanged = nameChanged
                || request.UpdateProgramDto.Description != program.Description
                || request.UpdateProgramDto.Location != program.Location
                || request.UpdateProgramDto.ParticipantsCount != program.ParticipantsCount
                || request.UpdateProgramDto.MeetingsCount != program.MeetingsCount;

            _mapper.Map(request.UpdateProgramDto, program);

            if (nameChanged || program.Slug is null)
            {
                var newSlug = await _slugService.GenerateUniqueHippotherapyProgramSlugAsync(program.Id, program.Name, cancellationToken);
                program.Slug = newSlug;
            }
            else
            {
                program.Slug = oldSlug;
            }

            var assignImagesResult = await ImageValidationHelper.ValidateAndAssignProgramImagesAsync(
                    _repositoryWrapper, program);

            if (assignImagesResult.IsFailed)
            {
                return Result.Fail<HippotherapyProgramDto>(assignImagesResult.Errors);
            }

            var categoriesChenged = program.Categories.Select(c => c.Id).OrderBy(id => id)
                .SequenceEqual(request.UpdateProgramDto.CategoryIds.OrderBy(id => id)) == false;

            if (categoriesChenged)
            {
                ReplaceCategories(program, newCategoriesResult.Value);
            }

            var now = DateTimeOffset.UtcNow;

            var orphanedFaqQuestions = GetOrphanedFaqQuestions(program, request.UpdateProgramDto.Sections);

            using var transaction = _repositoryWrapper.BeginTransaction();

            var oldSections = program.Sections.ToList();

            var sectionAdded = MergeSections(
                program,
                oldSections,
                request.UpdateProgramDto.Sections,
                now,
                imagesByIdResult.Value,
                out var programContentsChanged);

            if (sectionAdded)
            {
                MarkProgramLocalizationsOutdated(program);
            }

            if (programFieldsChanged || programContentsChanged)
            {
                SerTranslationsToOutdated(program);
            }

            _repositoryWrapper.HippotherapyProgramsRepository.Update(program);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<HippotherapyProgramDto>(
                    ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyProgram)));
            }

            if (orphanedFaqQuestions.Count > 0)
            {
                _repositoryWrapper.FaqQuestionsRepository.DeleteRange(orphanedFaqQuestions);
                await _repositoryWrapper.SaveChangesAsync();
            }

            var assignFaqQuestionsResult = await FaqQuestionHelper
                .AssignSectionContentFaqQuestionsAsync(_repositoryWrapper, program.Sections);

            if (assignFaqQuestionsResult.IsFailed)
            {
                return Result.Fail<HippotherapyProgramDto>(assignFaqQuestionsResult.Errors);
            }

            transaction.Complete();

            return Result.Ok(_mapper.Map<HippotherapyProgramDto>(program));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyProgramDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
    }

    private static void SerTranslationsToOutdated(HippotherapyProgram program)
    {
        MarkProgramLocalizationsOutdated(program);

        var sections = program.Sections.ToList();
        var contentsToOutdated = sections
            .SelectMany(s => s.Contents
                .SelectMany(c => c.Localizations))
            .ToList();

        foreach (var content in contentsToOutdated)
        {
            content.TranslationStatus = TranslationStatus.Outdated;
        }
    }

    private static void ReplaceCategories(HippotherapyProgram program, ICollection<HippotherapyProgramCategory> categories)
    {
        program.Categories.Clear();
        foreach (var category in categories)
        {
            program.Categories.Add(category);
        }
    }

    private static void MarkProgramLocalizationsOutdated(ITranslatedEntity<HippotherapyProgramLocalization> program)
    {
        foreach (var loc in program.Localizations)
        {
            loc.TranslationStatus = TranslationStatus.Outdated;
        }
    }

    private static bool MergeSections(
        HippotherapyProgram program,
        List<HippotherapyProgramSection> oldSections,
        List<CreateHippotherapyProgramSectionDto> newSections,
        DateTimeOffset now,
        IReadOnlyDictionary<long, Image> imagesById,
        out bool anyContentChanged)
    {
        anyContentChanged = false;
        var sectionAdded = false;

        var oldSectionsById = oldSections
            .Where(s => s.Id > 0)
            .ToDictionary(s => s.Id);

        var matchedOldSectionIds = new HashSet<long>();

        foreach (var newSectionDto in newSections)
        {
            var existingSection = newSectionDto.Id is > 0
                && oldSectionsById.TryGetValue(newSectionDto.Id.Value, out var found)
                ? found
                : null;

            if (existingSection is not null)
            {
                matchedOldSectionIds.Add(existingSection.Id);
                existingSection.Template = newSectionDto.Template;
                existingSection.Order = newSectionDto.Order;

                MergeSectionContents(existingSection, newSectionDto, now, imagesById, ref anyContentChanged);
            }
            else
            {
                var builtSections = HippotherapyProgramSectionsBuilder.Build(
                    [newSectionDto],
                    now,
                    imagesById);

                foreach (var built in builtSections)
                {
                    program.Sections.Add(built);
                }

                sectionAdded = true;
            }
        }

        var sectionsToRemove = oldSections
            .Where(s => !matchedOldSectionIds.Contains(s.Id))
            .ToList();

        foreach (var sectionToRemove in sectionsToRemove)
        {
            program.Sections.Remove(sectionToRemove);
        }

        return sectionAdded;
    }

    private static void MergeSectionContents(
        HippotherapyProgramSection existingSection,
        CreateHippotherapyProgramSectionDto newSectionDto,
        DateTimeOffset now,
        IReadOnlyDictionary<long, Image> imagesById,
        ref bool anyContentChanged)
    {
        var newContents = newSectionDto.Contents ?? [];

        var oldContentsById = existingSection.Contents
            .Where(c => c.Id > 0)
            .ToDictionary(c => c.Id);

        var matchedOldContentIds = new HashSet<long>();

        foreach (var newContentDto in newContents.OrderBy(c => c.Order))
        {
            var existingContent = FindMatchingContent(newContentDto, oldContentsById);

            if (existingContent is not null)
            {
                matchedOldContentIds.Add(existingContent.Id);
                ApplyExistingContentUpdate(existingContent, newContentDto, imagesById, ref anyContentChanged);
            }
            else
            {
                var newContent = HippotherapyProgramSectionsBuilder.CreateContent(newContentDto, imagesById, now);
                if (newContent is not null)
                {
                    existingSection.Contents.Add(newContent);
                }
            }
        }

        RemoveUnmatchedContents(existingSection, matchedOldContentIds);
    }

    private static ProgramSectionContent? FindMatchingContent(
        CreateProgramSectionContentDto newContentDto,
        IDictionary<long, ProgramSectionContent> oldContentsById)
    {
        if (newContentDto.Id is > 0
            && oldContentsById.TryGetValue(newContentDto.Id.Value, out var found)
            && found.ContentType == newContentDto.ContentType)
        {
            return found;
        }

        return null;
    }

    private static void ApplyExistingContentUpdate(
        ProgramSectionContent existingContent,
        CreateProgramSectionContentDto newContentDto,
        IReadOnlyDictionary<long, Image> imagesById,
        ref bool anyContentChanged)
    {
        var updated = TryApplyContentFieldUpdates(existingContent, newContentDto, imagesById, out var contentChanged);

        if (!updated || !contentChanged || existingContent.ContentType == ContentType.Image)
        {
            return;
        }

        anyContentChanged = true;

        foreach (var loc in existingContent.Localizations)
        {
            loc.TranslationStatus = TranslationStatus.Outdated;
        }
    }

    private static void RemoveUnmatchedContents(
        HippotherapyProgramSection existingSection,
        ICollection<long> matchedOldContentIds)
    {
        var contentsToRemove = existingSection.Contents
            .Where(c => c.Id > 0 && !matchedOldContentIds.Contains(c.Id))
            .ToList();

        foreach (var contentToRemove in contentsToRemove)
        {
            existingSection.Contents.Remove(contentToRemove);
        }
    }

    private static bool TryApplyContentFieldUpdates(
        ProgramSectionContent oldContent,
        CreateProgramSectionContentDto newContent,
        IReadOnlyDictionary<long, Image> imagesById,
        out bool contentChanged)
    {
        contentChanged = false;
        oldContent.Order = newContent.Order;
        oldContent.GroupIndex = newContent.GroupIndex;

        return newContent.ContentType switch
        {
            ContentType.Title when oldContent is TitleProgramContent titleContent
                => UpdateTitleContent(titleContent, newContent, out contentChanged),
            ContentType.Description when oldContent is DescriptionProgramContent descriptionContent
                => UpdateDescriptionContent(descriptionContent, newContent, out contentChanged),
            ContentType.Image when oldContent is ImageProgramContent imageContent
                => UpdateImageContent(imageContent, newContent, imagesById, out contentChanged),
            ContentType.Author when oldContent is AuthorProgramContent authorContent
                => UpdateAuthorContent(authorContent, newContent, out contentChanged),
            ContentType.FaqQuestion when oldContent is FaqQuestionProgramContent faqContent
                => UpdateFaqQuestionContent(faqContent, newContent, out contentChanged),
            _ => false,
        };
    }

    private static bool UpdateTitleContent(
        TitleProgramContent content,
        CreateProgramSectionContentDto source,
        out bool changed)
    {
        changed = false;
        if (source.Title is null)
        {
            return false;
        }

        var newValue = source.Title.Trim();
        changed = !string.Equals(content.Title, newValue, StringComparison.Ordinal);
        content.Title = newValue;
        return true;
    }

    private static bool UpdateDescriptionContent(
        DescriptionProgramContent content,
        CreateProgramSectionContentDto source,
        out bool changed)
    {
        changed = false;
        if (source.Description is null)
        {
            return false;
        }

        var newValue = source.Description.Trim();
        changed = !string.Equals(content.Description, newValue, StringComparison.Ordinal);
        content.Description = newValue;
        return true;
    }

    private static bool UpdateImageContent(
        ImageProgramContent content,
        CreateProgramSectionContentDto source,
        IReadOnlyDictionary<long, Image> imagesById,
        out bool changed)
    {
        changed = false;
        if (source.ImageId is null || !imagesById.TryGetValue(source.ImageId.Value, out var image))
        {
            return false;
        }

        changed = false;
        content.ImageId = source.ImageId.Value;
        content.Image = image;
        return true;
    }

    private static bool UpdateAuthorContent(
        AuthorProgramContent content,
        CreateProgramSectionContentDto source,
        out bool changed)
    {
        changed = false;
        if (source.Author is null)
        {
            return false;
        }

        var newValue = source.Author.Trim();
        changed = !string.Equals(content.Name, newValue, StringComparison.Ordinal);
        content.Name = newValue;
        return true;
    }

    private static bool UpdateFaqQuestionContent(
        FaqQuestionProgramContent content,
        CreateProgramSectionContentDto source,
        out bool changed)
    {
        changed = false;
        if (source.FaqQuestion is null || content.FaqQuestion is null)
        {
            return true;
        }

        var newQuestion = source.FaqQuestion.QuestionText?.Trim() ?? string.Empty;
        var newAnswer = source.FaqQuestion.AnswerText?.Trim() ?? string.Empty;

        changed = !string.Equals(content.FaqQuestion.QuestionText, newQuestion, StringComparison.Ordinal)
            || !string.Equals(content.FaqQuestion.AnswerText, newAnswer, StringComparison.Ordinal);

        content.FaqQuestion.QuestionText = newQuestion;
        content.FaqQuestion.AnswerText = newAnswer;
        return true;
    }

    private static List<FaqQuestion> GetOrphanedFaqQuestions(
        HippotherapyProgram program,
        ICollection<CreateHippotherapyProgramSectionDto>? incomingSections)
    {
        var incomingIds = (incomingSections ?? [])
            .SelectMany(s => s.Contents ?? [])
            .Where(c => c.ContentType == ContentType.FaqQuestion)
            .Select(c => c.FaqQuestion?.Id)
            .Where(id => id is > 0)
            .Select(id => id!.Value)
            .ToHashSet();

        return program.Sections
            .SelectMany(s => s.Contents)
            .OfType<FaqQuestionProgramContent>()
            .Where(c => c.FaqQuestion is not null && !incomingIds.Contains(c.FaqQuestionId))
            .Select(c => c.FaqQuestion!)
            .ToList();
    }

    private static void UpdateExistingFaqQuestionTexts(
        ICollection<HippotherapyProgramSection> sections,
        ICollection<CreateHippotherapyProgramSectionDto>? incomingSections)
    {
        var incomingById = (incomingSections ?? [])
            .SelectMany(s => s.Contents ?? [])
            .Where(c => c.ContentType == ContentType.FaqQuestion
                && c.FaqQuestion?.Id is > 0)
            .ToDictionary(c => c.FaqQuestion!.Id!.Value, c => c.FaqQuestion!);

        if (incomingById.Count == 0)
        {
            return;
        }

        foreach (var content in sections
            .SelectMany(s => s.Contents ?? [])
            .OfType<FaqQuestionProgramContent>()
            .Where(c => c.FaqQuestion is not null && incomingById.ContainsKey(c.FaqQuestionId)))
        {
            var dto = incomingById[content.FaqQuestionId];
            content.FaqQuestion!.QuestionText = dto.QuestionText.Trim();
            content.FaqQuestion!.AnswerText = dto.AnswerText.Trim();
        }
    }
}
