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

namespace VictoryCenter.BLL.Commands.Admin.Localization.FaqQuestions.Update;

public class UpdateFaqQuestionLocalizationHandler : IRequestHandler<UpdateFaqQuestionLocalizationCommand, Result<FaqQuestionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateFaqQuestionLocalizationCommand> _validator;
    private readonly ILocalizationService<FaqQuestion, FaqQuestionLocalization> _localizationService;

    public UpdateFaqQuestionLocalizationHandler(
        IMapper mapper,
        IValidator<UpdateFaqQuestionLocalizationCommand> validator,
        ILocalizationService<FaqQuestion, FaqQuestionLocalization> localizationService)
    {
        _mapper = mapper;
        _validator = validator;
        _localizationService = localizationService;
    }

    public async Task<Result<FaqQuestionLocalizationDto>> Handle(UpdateFaqQuestionLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var dto = request.UpdateFaqQuestionLocalizationDto;
            FaqQuestionLocalization entity = _mapper.Map<FaqQuestionLocalization>(dto);
            entity.EntityId = request.EntityId;
            entity.LanguageId = request.LanguageId;
            var result = await _localizationService.UpdateEntityLocalizationAsync(entity);
            FaqQuestionLocalizationDto responseDto = _mapper.Map<FaqQuestionLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<FaqQuestionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<FaqQuestionLocalizationDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(FaqQuestionLocalization)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<FaqQuestionLocalizationDto>(ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<FaqQuestionLocalizationDto>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(FaqQuestionLocalization)));
        }
    }
}
