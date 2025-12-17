using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FaqQuestions;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.BLL.Commands.Admin.Localization.FaqQuestions.Delete;

public class DeleteFaqQuestionLocalizationHandler : IRequestHandler<DeleteFaqQuestionLocalizationCommand, Result<DeleteFaqQuestionLocalizationDto>>
{
    private readonly ILocalizationService<FaqQuestion, FaqQuestionLocalization> _localizationService;

    public DeleteFaqQuestionLocalizationHandler(ILocalizationService<FaqQuestion, FaqQuestionLocalization> localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<Result<DeleteFaqQuestionLocalizationDto>> Handle(DeleteFaqQuestionLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var (entityId, languageId) = await _localizationService.DeleteEntityLocalizationAsync(request.EntityId, request.LanguageId);
            return Result.Ok(new DeleteFaqQuestionLocalizationDto { EntityId = entityId, LanguageId = languageId });
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<DeleteFaqQuestionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail(ErrorMessagesConstants.FailedToDeleteEntity(typeof(FaqQuestionLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<DeleteFaqQuestionLocalizationDto>(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(FaqQuestionLocalization)));
        }
    }
}
