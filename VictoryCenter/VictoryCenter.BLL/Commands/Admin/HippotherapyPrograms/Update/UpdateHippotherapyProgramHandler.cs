using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Update;

public class UpdateHippotherapyProgramHandler : IRequestHandler<UpdateHippotherapyProgramCommand, Result<HippotherapyProgramDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateHippotherapyProgramCommand> _validator;

    public UpdateHippotherapyProgramHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateHippotherapyProgramCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
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

            var imagesByIdResult = await GetSectionImagesAsync(request);

            if (imagesByIdResult.IsFailed)
            {
                return Result.Fail<HippotherapyProgramDto>(imagesByIdResult.Errors);
            }

            _mapper.Map(request.UpdateProgramDto, program);

            var assignImagesResult = await AssignProgramImagesAsync(program);

            if (assignImagesResult.IsFailed)
            {
                return Result.Fail<HippotherapyProgramDto>(assignImagesResult.Errors);
            }

            ReplaceCategories(program, newCategoriesResult.Value);

            var now = DateTimeOffset.UtcNow;

            ReplaceSections(program, request.UpdateProgramDto.Sections, now, imagesByIdResult.Value);

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

    private async Task<Result<IReadOnlyDictionary<long, Image>>> GetSectionImagesAsync(UpdateHippotherapyProgramCommand request)
    {
        var sectionImageIds = (request.UpdateProgramDto.Sections ?? [])
            .SelectMany(s => s.ImageIds ?? []);

        return await ImageValidationHelper.ValidateAndGetImagesByIdsAsync(
            _repositoryWrapper,
            sectionImageIds);
    }

    private async Task<Result> AssignProgramImagesAsync(HippotherapyProgram program)
    {
        var backgroundImageResult = await ImageValidationHelper.ValidateAndGetImageAsync(
            _repositoryWrapper,
            program.BackgroundImageId);

        if (backgroundImageResult.IsFailed)
        {
            return Result.Fail(backgroundImageResult.Errors);
        }

        var previewImageResult = await ImageValidationHelper.ValidateAndGetImageAsync(
            _repositoryWrapper,
            program.PreviewImageId);

        if (previewImageResult.IsFailed)
        {
            return Result.Fail(previewImageResult.Errors);
        }

        program.BackgroundImage = backgroundImageResult.Value;
        program.PreviewImage = previewImageResult.Value;

        return Result.Ok();
    }

    private static void ReplaceCategories(HippotherapyProgram program, ICollection<HippotherapyProgramCategory> categories)
    {
        program.Categories.Clear();
        foreach (var category in categories)
        {
            program.Categories.Add(category);
        }
    }

    private static void ReplaceSections(
        HippotherapyProgram program,
        List<CreateHippotherapyProgramSectionDto>? sections,
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
