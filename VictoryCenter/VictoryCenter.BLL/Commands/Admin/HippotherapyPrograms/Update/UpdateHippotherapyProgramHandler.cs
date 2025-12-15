using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;
using VictoryCenter.BLL.Helpers;
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

    public UpdateHippotherapyProgramHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateHippotherapyProgramCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<HippotherapyProgramDto>> Handle(UpdateHippotherapyProgramCommand request, CancellationToken cancellationToken)
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

            var sectionImageIds = (request.UpdateProgramDto.Sections ?? [])
                .SelectMany(s => s.ImageIds ?? []);

            var imagesByIdsResult = await ImageValidationHelper.ValidateAndGetImagesByIdsAsync(
                _repositoryWrapper,
                sectionImageIds);

            if (imagesByIdsResult.IsFailed)
            {
                return Result.Fail<HippotherapyProgramDto>(imagesByIdsResult.Errors);
            }

            var imagesById = imagesByIdsResult.Value;

            _mapper.Map(request.UpdateProgramDto, program);

            var backgroundImageResult = await ImageValidationHelper.ValidateAndGetImageAsync(
                _repositoryWrapper,
                program.BackgroundImageId);

            if (backgroundImageResult.IsFailed)
            {
                return Result.Fail<HippotherapyProgramDto>(backgroundImageResult.Errors);
            }

            var previewImageResult = await ImageValidationHelper.ValidateAndGetImageAsync(
                _repositoryWrapper,
                program.PreviewImageId);

            if (previewImageResult.IsFailed)
            {
                return Result.Fail<HippotherapyProgramDto>(previewImageResult.Errors);
            }

            program.BackgroundImage = backgroundImageResult.Value;
            program.PreviewImage = previewImageResult.Value;

            program.Categories.Clear();
            foreach (var category in newCategoriesResult.Value)
            {
                program.Categories.Add(category);
            }

            program.Sections.Clear();
            AddSections(program, request.UpdateProgramDto.Sections, DateTimeOffset.UtcNow, imagesById);

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

    // TODO: Move to a separate file

    private static void AddSections(
        HippotherapyProgram program,
        List<CreateHippotherapyProgramSectionDto>? sections,
        DateTimeOffset now,
        IReadOnlyDictionary<long, Image> imagesById)
    {
        foreach (var sectionDto in sections ?? [])
        {
            program.Sections.Add(new HippotherapyProgramSection
            {
                Template = sectionDto.Template,
                Order = sectionDto.Order,
                CreatedAt = now,
                Contents = BuildContents(sectionDto, imagesById)
            });
        }
    }

    private static List<ProgramSectionContent> BuildContents(
        CreateHippotherapyProgramSectionDto sectionDto,
        IReadOnlyDictionary<long, Image> imagesById)
    {
        var contents = new List<ProgramSectionContent>(
            (sectionDto.Titles?.Count ?? 0) +
            (sectionDto.Descriptions?.Count ?? 0) +
            (sectionDto.ImageIds?.Count ?? 0));

        var order = 0;

        foreach (var title in sectionDto.Titles ?? [])
        {
            contents.Add(new TitleProgramContent
            {
                ContentType = ContentType.Title,
                Order = order++,
                Title = title.Trim()
            });
        }

        foreach (var description in sectionDto.Descriptions ?? [])
        {
            contents.Add(new DescriptionProgramContent
            {
                ContentType = ContentType.Description,
                Order = order++,
                Description = description.Trim()
            });
        }

        foreach (var imageId in sectionDto.ImageIds ?? [])
        {
            contents.Add(new ImageProgramContent
            {
                ContentType = ContentType.Image,
                Order = order++,
                ImageId = imageId,
                Image = imagesById[imageId]
            });
        }

        return contents;
    }
}
