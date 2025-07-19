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

namespace VictoryCenter.BLL.Commands.Admin.FaqQuestions.Create;

public class CreateFaqQuestionHandler : IRequestHandler<CreateFaqQuestionCommand, Result<FaqQuestionDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateFaqQuestionCommand> _validator;

    public CreateFaqQuestionHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<CreateFaqQuestionCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<FaqQuestionDto>> Handle(
        CreateFaqQuestionCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var allPages = await _repositoryWrapper.VisitorPagesRepository.GetAllAsync();
            FaqQuestion entity = _mapper.Map<FaqQuestion>(request.CreateFaqQuestionDto);

            foreach (var pageId in request.CreateFaqQuestionDto.PageIds)
            {
                if (!allPages.Any(p => p.Id == pageId))
                {
                    return Result.Fail<FaqQuestionDto>(ErrorMessagesConstants.NotFound(pageId, typeof(VisitorPage)));
                }

                // var maxPriority = await _repositoryWrapper.FaqPlacementsRepository.MaxAsync(
                //         place => place.Priority,
                //         place => place.PageId == pageId);

                // entity.Placements.Add(new FaqPlacement
                // {
                //     PageId = pageId,
                //     Priority = (maxPriority ?? 0) + 1
                // });

                var maxPriority = (await _repositoryWrapper.FaqPlacementsRepository
                    .GetAllAsync()).Where(p => p.PageId == pageId).MaxBy(p => p.Priority)?.Priority ?? 0;

                entity.Placements.Add(new FaqPlacement
                {
                    PageId = pageId,
                    Priority = maxPriority + 1
                });
            }

            entity.CreatedAt = DateTime.UtcNow;
            using TransactionScope scope = _repositoryWrapper.BeginTransaction();
            await _repositoryWrapper.FaqQuestionsRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                scope.Complete();
                FaqQuestionDto createdEntity = _mapper.Map<FaqQuestionDto>(entity);
                return Result.Ok(createdEntity);
            }

            return Result.Fail<FaqQuestionDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(FaqQuestion)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<FaqQuestionDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail<FaqQuestionDto>(ErrorMessagesConstants.
                FailedToCreateEntityInDatabase(typeof(FaqQuestion)) + ex.Message);
        }
    }
}
