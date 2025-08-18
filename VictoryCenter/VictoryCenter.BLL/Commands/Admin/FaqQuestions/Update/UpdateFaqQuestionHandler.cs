using System.Transactions;
using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.FaqQuestions.Update;

public class UpdateFaqQuestionHandler : IRequestHandler<UpdateFaqQuestionCommand, Result<FaqQuestionDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateFaqQuestionCommand> _validator;

    public UpdateFaqQuestionHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateFaqQuestionCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<FaqQuestionDto>> Handle(UpdateFaqQuestionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            FaqQuestion? faqQuestionEntity =
                await _repositoryWrapper.FaqQuestionsRepository.GetFirstOrDefaultAsync(new QueryOptions<FaqQuestion>
                {
                    Filter = entity => entity.Id == request.Id,
                    Include = e => e.Include(q => q.Placements),
                });

            if (faqQuestionEntity is null)
            {
                return Result.Fail<FaqQuestionDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(FaqQuestion)));
            }

            FaqQuestion? entityToUpdate = _mapper.Map<UpdateFaqQuestionDto, FaqQuestion>(request.UpdateFaqQuestionDto);
            entityToUpdate.Id = request.Id;
            entityToUpdate.CreatedAt = faqQuestionEntity.CreatedAt;

            using TransactionScope scope = _repositoryWrapper.BeginTransaction();

            int affectedRows = 0;

            _repositoryWrapper.FaqQuestionsRepository.Update(entityToUpdate);
            affectedRows += await _repositoryWrapper.SaveChangesAsync();

            var questionPlacements = (await _repositoryWrapper.FaqPlacementsRepository.GetAllAsync(
                new QueryOptions<FaqPlacement>
                {
                    Filter = fp => fp.QuestionId == request.Id,
                    OrderByASC = fp => fp.Priority,
                })).ToList();

            var allPageIds = (await _repositoryWrapper.VisitorPagesRepository.GetAllAsync()).Select(p => p.Id).ToList();
            var existingPageIds = questionPlacements.Select(p => p.PageId).ToList();
            var removedPageIds = existingPageIds.Except(request.UpdateFaqQuestionDto.PageIds).ToList();
            var addedPageIds = request.UpdateFaqQuestionDto.PageIds.Except(existingPageIds).ToList();

            if (removedPageIds.Count > 0)
            {
                var affectedPages = await _repositoryWrapper.FaqPlacementsRepository.GetAllAsync(
                    new QueryOptions<FaqPlacement>
                    {
                        Filter = fp => removedPageIds.Contains(fp.PageId),
                    });
                var pageGroups = affectedPages.GroupBy(p => p.PageId).ToList();

                var deleted = questionPlacements.Where(p => removedPageIds.Contains(p.PageId)).ToList();
                _repositoryWrapper.FaqPlacementsRepository.DeleteRange(deleted);
                affectedRows += await _repositoryWrapper.SaveChangesAsync();

                foreach (var id in removedPageIds)
                {
                    var question = questionPlacements.Single(q => q.PageId == id && q.QuestionId == request.Id);
                    var group = pageGroups
                        .Single(g => g.Key == id)
                        .OrderBy(q => q.Priority)
                        .Skip((int)question.Priority)
                        .ToList();

                    foreach (var faq in group)
                    {
                        faq.Priority = -(faq.Priority - 1);
                    }

                    _repositoryWrapper.FaqPlacementsRepository.UpdateRange(group);
                    affectedRows += await _repositoryWrapper.SaveChangesAsync();

                    foreach (var faq in group)
                    {
                        faq.Priority = -faq.Priority;
                    }
                }
            }

            if (addedPageIds.Count > 0)
            {
                foreach (var addedId in addedPageIds)
                {
                    if (!allPageIds.Contains(addedId))
                    {
                        return Result.Fail<FaqQuestionDto>(FaqConstants.SomePagesNotFound);
                    }
                }

                var affectedPages = await _repositoryWrapper.FaqPlacementsRepository.GetAllAsync(
                    new QueryOptions<FaqPlacement>
                    {
                        Filter = fp => addedPageIds.Contains(fp.PageId),
                    });
                var pageGroups = affectedPages.GroupBy(p => p.PageId).ToList();

                var newPlacements = new List<FaqPlacement>();

                foreach (var id in addedPageIds)
                {
                    var group = pageGroups.FirstOrDefault(g => g.Key == id)?.ToList();
                    var maxPriority = group != null ? group.Max(fp => fp.Priority) : 0;
                    var newPlacement = new FaqPlacement
                    {
                        PageId = id,
                        QuestionId = entityToUpdate.Id,
                        Priority = maxPriority + 1
                    };
                    newPlacements.Add(newPlacement);
                }

                await _repositoryWrapper.FaqPlacementsRepository.CreateRangeAsync(newPlacements);
                affectedRows += await _repositoryWrapper.SaveChangesAsync();
            }

            if (affectedRows > 0)
            {
                FaqQuestion? resultEntity =
                await _repositoryWrapper.FaqQuestionsRepository.GetFirstOrDefaultAsync(new QueryOptions<FaqQuestion>
                {
                    Filter = entity => entity.Id == request.Id,
                    Include = e => e.Include(q => q.Placements),
                });
                scope.Complete();
                FaqQuestionDto? resultDto = _mapper.Map<FaqQuestion, FaqQuestionDto>(resultEntity!);
                return Result.Ok(resultDto);
            }

            return Result.Fail<FaqQuestionDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(FaqQuestion)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<FaqQuestionDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (InvalidOperationException ex)
        {
            return Result.Fail<FaqQuestionDto>(ex.Message);
        }
    }
}
