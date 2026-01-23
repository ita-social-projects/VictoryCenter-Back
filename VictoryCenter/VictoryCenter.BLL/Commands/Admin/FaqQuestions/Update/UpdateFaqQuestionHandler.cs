using System.Transactions;
using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.FaqQuestions.Update;

public class UpdateFaqQuestionHandler : IRequestHandler<UpdateFaqQuestionCommand, Result<FaqQuestionDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateFaqQuestionCommand> _validator;
    private readonly IReorderService _reorderService;

    public UpdateFaqQuestionHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateFaqQuestionCommand> validator,
        IReorderService reorderService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _reorderService = reorderService;
    }

    public async Task<Result<FaqQuestionDto>> Handle(UpdateFaqQuestionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            FaqQuestion? entityToUpdate = await _repositoryWrapper.FaqQuestionsRepository.GetFirstOrDefaultAsync(
                new QueryOptions<FaqQuestion>
                {
                    Filter = entity => entity.Id == request.Id,
                    Include = e => e
                        .Include(q => q.Placements)
                        .Include(q => q.Localizations)
                            .ThenInclude(l => l.Language),
                });

            if (entityToUpdate is null)
            {
                return Result.Fail<FaqQuestionDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(FaqQuestion)));
            }

            using (TransactionScope scope = _repositoryWrapper.BeginTransaction())
            {
                int affectedRows = 0;
                SetTranslationsToOutdated(request, entityToUpdate);

                _mapper.Map(request.UpdateFaqQuestionDto, entityToUpdate);

                _repositoryWrapper.FaqQuestionsRepository.Update(entityToUpdate);
                affectedRows += await _repositoryWrapper.SaveChangesAsync();

                var questionPlacements = entityToUpdate.Placements.ToList();
                var allPageIds = (await _repositoryWrapper.VisitorPagesRepository.GetAllAsync()).Select(p => p.Id).ToList();
                var existingPageIds = questionPlacements.Select(p => p.PageId).ToList();
                var removedPageIds = existingPageIds.Except(request.UpdateFaqQuestionDto.PageIds).ToList();
                var addedPageIds = request.UpdateFaqQuestionDto.PageIds.Except(existingPageIds).ToList();

                if (removedPageIds.Count > 0)
                {
                    var deletedPlacements = questionPlacements.Where(p => removedPageIds.Contains(p.PageId)).ToList();
                    _repositoryWrapper.FaqPlacementsRepository.DeleteRange(deletedPlacements);
                    affectedRows += await _repositoryWrapper.SaveChangesAsync();

                    foreach (var pageId in removedPageIds)
                    {
                        await _reorderService.RenumberPriorityAsync<FaqPlacement>(
                            groupSelector: fp => fp.PageId == pageId);
                    }

                    affectedRows += await _repositoryWrapper.SaveChangesAsync();
                }

                if (addedPageIds.Count > 0)
                {
                    // Validate that all pages exist
                    var missingPageIds = addedPageIds.Except(allPageIds).ToList();
                    if (missingPageIds.Any())
                    {
                        return Result.Fail<FaqQuestionDto>(FaqConstants.SomePagesNotFound);
                    }

                    var newPlacements = new List<FaqPlacement>();

                    foreach (var pageId in addedPageIds)
                    {
                        var nextPriority = await _reorderService.GetNextDisplayOrderAsync<FaqPlacement>(fp => fp.PageId == pageId);
                        var newPlacement = new FaqPlacement
                        {
                            PageId = pageId,
                            QuestionId = entityToUpdate.Id,
                            Priority = nextPriority
                        };
                        newPlacements.Add(newPlacement);
                    }

                    await _repositoryWrapper.FaqPlacementsRepository.CreateRangeAsync(newPlacements);
                    affectedRows += await _repositoryWrapper.SaveChangesAsync();
                }

                if (affectedRows > 0)
                {
                    FaqQuestion? resultEntity = await _repositoryWrapper.FaqQuestionsRepository.GetFirstOrDefaultAsync(
                        new QueryOptions<FaqQuestion>
                        {
                            Filter = entity => entity.Id == request.Id,
                            Include = e => e.Include(q => q.Placements)
                            .Include(q => q.Localizations)
                                .ThenInclude(l => l.Language),
                        });

                    scope.Complete();
                    FaqQuestionDto? resultDto = _mapper.Map<FaqQuestion, FaqQuestionDto>(resultEntity!);
                    return Result.Ok(resultDto);
                }

                return Result.Fail<FaqQuestionDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(FaqQuestion)));
            }
        }
        catch (ValidationException vex)
        {
            return Result.Fail<FaqQuestionDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (Exception ex) when (ex is InvalidOperationException or DbUpdateException)
        {
            return Result.Fail<FaqQuestionDto>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(FaqQuestion)));
        }
    }

    private static void SetTranslationsToOutdated(UpdateFaqQuestionCommand request, FaqQuestion entityToUpdate)
    {
        if (!string.Equals(request.UpdateFaqQuestionDto.QuestionText, entityToUpdate.QuestionText) ||
            !string.Equals(request.UpdateFaqQuestionDto.AnswerText, entityToUpdate.AnswerText))
        {
            foreach (var loc in entityToUpdate.Localizations)
            {
                loc.TranslationStatus = TranslationStatus.Outdated;
            }
        }
    }
}
