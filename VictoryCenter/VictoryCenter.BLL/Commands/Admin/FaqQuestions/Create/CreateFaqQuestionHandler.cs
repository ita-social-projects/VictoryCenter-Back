using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.FaqQuestions.Create;

public class CreateFaqQuestionHandler : IRequestHandler<CreateFaqQuestionCommand, Result<FaqQuestionDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IReorderService _reorderService;
    private readonly IValidator<CreateFaqQuestionCommand> _validator;

    public CreateFaqQuestionHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IValidator<CreateFaqQuestionCommand> validator,
        IReorderService reorderService)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
        _reorderService = reorderService;
    }

    public async Task<Result<FaqQuestionDto>> Handle(
        CreateFaqQuestionCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            using var scope = _repositoryWrapper.BeginTransaction();

            var allPages = await _repositoryWrapper.VisitorPagesRepository.GetAllAsync();
            FaqQuestion entity = _mapper.Map<FaqQuestion>(request.CreateFaqQuestionDto);

            foreach (var pageId in request.CreateFaqQuestionDto.PageIds.Distinct())
            {
                if (!allPages.Any(p => p.Id == pageId))
                {
                    return Result.Fail<FaqQuestionDto>(ErrorMessagesConstants.NotFound(pageId, typeof(VisitorPage)));
                }

                var priority = await _reorderService.GetNextDisplayOrderAsync<FaqPlacement>(
                    groupSelector: fp => fp.PageId == pageId);

                entity.Placements.Add(new FaqPlacement
                {
                    PageId = pageId,
                    Priority = priority
                });
            }

            entity.CreatedAt = DateTime.UtcNow;

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
        catch (DbUpdateException)
        {
            return Result.Fail<FaqQuestionDto>(ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(FaqQuestion)));
        }
    }
}
