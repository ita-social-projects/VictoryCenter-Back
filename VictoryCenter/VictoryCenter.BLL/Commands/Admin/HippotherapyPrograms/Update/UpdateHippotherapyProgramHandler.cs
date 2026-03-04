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
                        .Include(x => x.Localizations)
                });

            if (program is null)
            {
                return Result.Fail<HippotherapyProgramDto>(
                    ErrorMessagesConstants.NotFound(request.Id, typeof(HippotherapyProgram)));
            }

            var newCategoriesResult = await CategoryValidationHelper.ValidateAndGetCategoriesAsync(
                _repositoryWrapper,
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

            foreach (var loc in program.Localizations)
            {
                if (programFieldsChanged)
                {
                    loc.TranslationStatus = TranslationStatus.Outdated;
                }
            }

            var categoriesChenged = program.Categories.Select(c => c.Id).OrderBy(id => id)
                .SequenceEqual(request.UpdateProgramDto.CategoryIds.OrderBy(id => id)) == false;

            if (categoriesChenged)
            {
                ReplaceCategories(program, newCategoriesResult.Value);
            }

            var now = DateTimeOffset.UtcNow;

            if (!EnsureReplaceSameSections(program.Sections.ToList(), request.UpdateProgramDto.Sections, imagesByIdResult.Value))
            {
                ReplaceSections(program, request.UpdateProgramDto.Sections, now, imagesByIdResult.Value);
                program.Localizations.Clear();
            }

            _repositoryWrapper.HippotherapyProgramsRepository.Update(program);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(_mapper.Map<HippotherapyProgramDto>(program));
            }

            return Result.Fail<HippotherapyProgramDto>(
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyProgram)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyProgramDto>(vex.Errors.Select(e => e.ErrorMessage));
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

    private static bool EnsureReplaceSameSections(
        List<HippotherapyProgramSection> oldSections,
        List<CreateHippotherapyProgramSectionDto> newSections,
        IReadOnlyDictionary<long, Image> imagesById)
    {
        if (oldSections.Count != newSections.Count)
        {
            return false;
        }

        Dictionary<int, HippotherapyProgramSection> oldSectionsMap;
        Dictionary<int, CreateHippotherapyProgramSectionDto> newSectionsMap;

        try
        {
            oldSectionsMap = oldSections.ToDictionary(section => section.Order);
            newSectionsMap = newSections.ToDictionary(section => section.Order);
        }
        catch (ArgumentException)
        {
            return false;
        }

        if (oldSectionsMap.Count != newSectionsMap.Count)
        {
            return false;
        }

        foreach (var (sectionOrder, newSection) in newSectionsMap)
        {
            if (!oldSectionsMap.TryGetValue(sectionOrder, out var oldSection))
            {
                return false;
            }

            if (oldSection.Template != newSection.Template)
            {
                return false;
            }

            var newContents = newSection.Contents ?? [];
            var oldContents = oldSection.Contents;

            Dictionary<int, CreateProgramSectionContentDto> newContentsMap;
            Dictionary<int, ProgramSectionContent> oldContentsMap;

            try
            {
                newContentsMap = newContents.ToDictionary(content => content.Order);
                oldContentsMap = oldContents.ToDictionary(content => content.Order);
            }
            catch (ArgumentException)
            {
                return false;
            }

            if (newContentsMap.Count != oldContentsMap.Count)
            {
                return false;
            }

            foreach (var (contentOrder, newContent) in newContentsMap)
            {
                if (!oldContentsMap.TryGetValue(contentOrder, out var oldContent))
                {
                    return false;
                }

                if (newContent.ContentType != oldContent.ContentType)
                {
                    return false;
                }

                if (!ApplyContentFieldUpdates(oldContent, newContent, imagesById))
                {
                    return false;
                }

                foreach (var loc in oldContent.Localizations)
                {
                    loc.TranslationStatus = TranslationStatus.Outdated;
                }
            }
        }

        return true;
    }

    private static bool ApplyContentFieldUpdates(
        ProgramSectionContent oldContent,
        CreateProgramSectionContentDto newContent,
        IReadOnlyDictionary<long, Image> imagesById)
    {
        oldContent.GroupIndex = newContent.GroupIndex;

        return newContent.ContentType switch
        {
            DAL.Enums.ContentType.Title when oldContent is TitleProgramContent titleContent
                => UpdateTitleContent(titleContent, newContent),
            DAL.Enums.ContentType.Description when oldContent is DescriptionProgramContent descriptionContent
                => UpdateDescriptionContent(descriptionContent, newContent),
            DAL.Enums.ContentType.Image when oldContent is ImageProgramContent imageContent
                => UpdateImageContent(imageContent, newContent, imagesById),
            DAL.Enums.ContentType.Author when oldContent is AuthorProgramContent authorContent
                => UpdateAuthorContent(authorContent, newContent),
            DAL.Enums.ContentType.Question when oldContent is QuestionProgramContent questionContent
                => UpdateQuestionContent(questionContent, newContent),
            DAL.Enums.ContentType.Answer when oldContent is AnswerProgramContent answerContent
                => UpdateAnswerContent(answerContent, newContent),
            _ => false,
        };
    }

    private static bool UpdateTitleContent(TitleProgramContent content, CreateProgramSectionContentDto source)
    {
        if (source.Title is null)
        {
            return false;
        }

        content.Title = source.Title.Trim();
        return true;
    }

    private static bool UpdateDescriptionContent(DescriptionProgramContent content, CreateProgramSectionContentDto source)
    {
        if (source.Description is null)
        {
            return false;
        }

        content.Description = source.Description.Trim();
        return true;
    }

    private static bool UpdateImageContent(
        ImageProgramContent content,
        CreateProgramSectionContentDto source,
        IReadOnlyDictionary<long, Image> imagesById)
    {
        if (source.ImageId is null || !imagesById.TryGetValue(source.ImageId.Value, out var image))
        {
            return false;
        }

        content.ImageId = source.ImageId.Value;
        content.Image = image;
        return true;
    }

    private static bool UpdateAuthorContent(AuthorProgramContent content, CreateProgramSectionContentDto source)
    {
        if (source.Author is null)
        {
            return false;
        }

        content.Name = source.Author.Trim();
        return true;
    }

    private static bool UpdateQuestionContent(QuestionProgramContent content, CreateProgramSectionContentDto source)
    {
        if (source.Question is null)
        {
            return false;
        }

        content.Question = source.Question.Trim();
        return true;
    }

    private static bool UpdateAnswerContent(AnswerProgramContent content, CreateProgramSectionContentDto source)
    {
        if (source.Answer is null)
        {
            return false;
        }

        content.Answer = source.Answer.Trim();
        return true;
    }

    private static void ReplaceSections(
        HippotherapyProgram program,
        ICollection<CreateHippotherapyProgramSectionDto>? sections,
        DateTimeOffset createdAt,
        IReadOnlyDictionary<long, Image> imagesById)
    {
        program.Sections.Clear();

        var builtSections = HippotherapyProgramSectionsBuilder.Build(
            sections,
            createdAt,
            imagesById);

        foreach (var section in builtSections)
        {
            program.Sections.Add(section);
        }
    }
}
