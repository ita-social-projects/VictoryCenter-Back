using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FaqQuestions;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.BLL.Commands.Admin.Localization.FaqQuestions.Create;

public class CreateFaqQuestionLocalizationHandler : IRequestHandler<CreateFaqQuestionLocalizationCommand, Result<FaqQuestionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IValidator<CreateFaqQuestionLocalizationCommand> _validator;
    private readonly ILocalizationService<FaqQuestion, FaqQuestionLocalization> _localizationService;

    public CreateFaqQuestionLocalizationHandler(
        IMapper mapper,
        IValidator<CreateFaqQuestionLocalizationCommand> validator,
        ILocalizationService<FaqQuestion, FaqQuestionLocalization> localizationService)
    {
        _mapper = mapper;
        _validator = validator;
        _localizationService = localizationService;
    }

    public async Task<Result<FaqQuestionLocalizationDto>> Handle(CreateFaqQuestionLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            FaqQuestionLocalization entity = _mapper.Map<FaqQuestionLocalization>(request.CreateFaqQuestionLocalizationDto);
            var result = await _localizationService.CreateEntityLocalizationAsync(entity);
            FaqQuestionLocalizationDto responseDto = _mapper.Map<FaqQuestionLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<FaqQuestionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<FaqQuestionLocalizationDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(FaqQuestionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<FaqQuestionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<FaqQuestionLocalizationDto>(ErrorMessagesConstants.
                FailedToCreateEntityInDatabase(typeof(FaqQuestionLocalization)));
        }
    }
}
