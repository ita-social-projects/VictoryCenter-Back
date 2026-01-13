using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Interfaces.SlugService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Create;

public class CreateHippotherapyProgramHandler
    : IRequestHandler<CreateHippotherapyProgramCommand, Result<HippotherapyProgramDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateHippotherapyProgramCommand> _validator;
    private readonly ISlugService _slugService;

    public CreateHippotherapyProgramHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<CreateHippotherapyProgramCommand> validator,
        ISlugService slugService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _slugService = slugService;
    }

    public async Task<Result<HippotherapyProgramDto>> Handle(
        CreateHippotherapyProgramCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var categoriesResult = await CategoryValidationHelper.ValidateAndGetCategoriesAsync(
                _repositoryWrapper,
                request.CreateProgramDto.CategoryIds);

            if (categoriesResult.IsFailed)
            {
                return Result.Fail<HippotherapyProgramDto>(categoriesResult.Errors);
            }

            var imagesByIdResult = await ImageValidationHelper.ValidateAndGetSectionImagesAsync(
                _repositoryWrapper, request.CreateProgramDto.Sections);

            if (imagesByIdResult.IsFailed)
            {
                return Result.Fail<HippotherapyProgramDto>(imagesByIdResult.Errors);
            }

            var program = _mapper.Map<HippotherapyProgram>(request.CreateProgramDto);

            var slugFromTitle = await _slugService.GenerateUniqueHippotherapyProgramSlugAsync(program.Id, request.CreateProgramDto.Name, cancellationToken);

            program.Slug = slugFromTitle;

            var assignImagesResult = await ImageValidationHelper.ValidateAndAssignProgramImagesAsync(
                _repositoryWrapper, program);

            if (assignImagesResult.IsFailed)
            {
                return Result.Fail<HippotherapyProgramDto>(assignImagesResult.Errors);
            }

            AddCategories(program, categoriesResult.Value);

            var now = DateTimeOffset.UtcNow;
            program.CreatedAt = now;

            AddSections(program, request.CreateProgramDto.Sections, now, imagesByIdResult.Value);

            await _repositoryWrapper.HippotherapyProgramsRepository.CreateAsync(program);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(_mapper.Map<HippotherapyProgramDto>(program));
            }

            return Result.Fail<HippotherapyProgramDto>(
                ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyProgram)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyProgramDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
    }

    private static void AddCategories(HippotherapyProgram program, ICollection<HippotherapyProgramCategory> categories)
    {
        foreach (var category in categories)
        {
            program.Categories.Add(category);
        }
    }

    private static void AddSections(
        HippotherapyProgram program,
        ICollection<CreateHippotherapyProgramSectionDto>? sections,
        DateTimeOffset createdAt,
        IReadOnlyDictionary<long, Image> imagesById)
    {
        var builtSections = HippotherapyProgramSectionsBuilder.Build(sections, createdAt, imagesById);

        foreach (var section in builtSections)
        {
            program.Sections.Add(section);
        }
    }
}
