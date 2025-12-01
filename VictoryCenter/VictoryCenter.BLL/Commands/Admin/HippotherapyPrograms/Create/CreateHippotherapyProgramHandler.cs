using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Create;

public class CreateHippotherapyProgramHandler : IRequestHandler<CreateHippotherapyProgramCommand, Result<HippotherapyProgramDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateHippotherapyProgramCommand> _validator;

    public CreateHippotherapyProgramHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<CreateHippotherapyProgramCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
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

            var program = _mapper.Map<HippotherapyProgram>(request.CreateProgramDto);

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

            foreach (var category in categoriesResult.Value)
            {
                program.Categories.Add(category);
            }

            program.CreatedAt = DateTimeOffset.UtcNow;

            await _repositoryWrapper.HippotherapyProgramsRepository.CreateAsync(program);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(_mapper.Map<HippotherapyProgramDto>(program));
            }

            return Result.Fail<HippotherapyProgramDto>(
                ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyProgram)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<HippotherapyProgramDto>(ex.Message);
        }
    }
}
