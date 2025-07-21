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
                return Result.Fail<FaqQuestionDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(FaqQuestionDto)));
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

            var pageIds = questionPlacements.Select(fp => fp.PageId).ToList();
            var deletedIds = pageIds.Except(request.UpdateFaqQuestionDto.PageIds).ToList();
            var addedIds = request.UpdateFaqQuestionDto.PageIds.Except(pageIds).ToList();

            if (deletedIds.Count > 0)
            {
                var affectedPages = await _repositoryWrapper.FaqPlacementsRepository.GetAllAsync(
                    new QueryOptions<FaqPlacement>
                    {
                        Filter = fp => deletedIds.Contains(fp.PageId),
                    });
                var pageGroups = affectedPages.GroupBy(p => p.PageId).ToList();

                var deleted = questionPlacements.Where(p => deletedIds.Contains(p.PageId)).ToList();
                _repositoryWrapper.FaqPlacementsRepository.DeleteRange(deleted);
                affectedRows += await _repositoryWrapper.SaveChangesAsync();

                var t = (await _repositoryWrapper.FaqPlacementsRepository.GetAllAsync()).Where(x => x.PageId == 3).ToList();

                foreach (var id in deletedIds)
                {
                    var group = pageGroups.FirstOrDefault(g => g.Key == id)?.ToList() ?? throw new InvalidOperationException();
                    var question = questionPlacements.FirstOrDefault(q => q.PageId == id) ?? throw new InvalidOperationException();

                    for (int i = (int)question.Priority; i < group.Count; i++)
                    {
                        // Workaround to avoid duplicate values
                        group[i].Priority = -(group[i].Priority - 1);
                    }

                    _repositoryWrapper.FaqPlacementsRepository.UpdateRange(group.Skip((int)question.Priority));
                    affectedRows += await _repositoryWrapper.SaveChangesAsync();

                    for (int i = (int)question.Priority; i < group.Count; i++)
                    {
                        // Get back to the normal state
                        group[i].Priority = -group[i].Priority;
                    }

                    _repositoryWrapper.FaqPlacementsRepository.UpdateRange(group.Skip((int)question.Priority));
                    affectedRows += await _repositoryWrapper.SaveChangesAsync();
                }
            }

            if (addedIds.Count > 0)
            {
                var affectedPages = await _repositoryWrapper.FaqPlacementsRepository.GetAllAsync(
                    new QueryOptions<FaqPlacement>
                    {
                        Filter = fp => addedIds.Contains(fp.PageId),
                    });
                var pageGroups = affectedPages.GroupBy(p => p.PageId).ToList();

                var newPlacements = new List<FaqPlacement>();

                foreach (var id in addedIds)
                {
                    var group = pageGroups.FirstOrDefault(g => g.Key == id)?.ToList() ?? throw new InvalidOperationException();
                    var maxPriority = group.Max(fp => fp.Priority);
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
    }
}
