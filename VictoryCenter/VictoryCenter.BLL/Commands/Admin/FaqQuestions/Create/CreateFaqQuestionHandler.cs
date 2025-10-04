using System.Transactions;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.FaqQuestions.Create;

public class CreateFaqQuestionHandler : BaseHandler<CreateFaqQuestionCommand, FaqQuestionDto>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateFaqQuestionCommand> _validator;

    public CreateFaqQuestionHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IValidator<CreateFaqQuestionCommand> validator)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
    }

    public override async Task<FaqQuestionDto> HandleRequest(CreateFaqQuestionCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var allPages = await _repositoryWrapper.VisitorPagesRepository.GetAllAsync();
        FaqQuestion entity = _mapper.Map<FaqQuestion>(request.CreateFaqQuestionDto);

        foreach (var pageId in request.CreateFaqQuestionDto.PageIds)
        {
            if (!allPages.Any(p => p.Id == pageId))
            {
                throw new Exception(ErrorMessagesConstants.NotFound(pageId, typeof(VisitorPage)));
            }

            var maxPriority = await _repositoryWrapper.FaqPlacementsRepository.MaxAsync(
                    place => place.Priority,
                    place => place.PageId == pageId);

            entity.Placements.Add(new FaqPlacement
            {
                PageId = pageId,
                Priority = (maxPriority ?? 0) + 1
            });
        }

        entity.CreatedAt = DateTime.UtcNow;
        using TransactionScope scope = _repositoryWrapper.BeginTransaction();
        await _repositoryWrapper.FaqQuestionsRepository.CreateAsync(entity);

        if (await _repositoryWrapper.SaveChangesAsync() <= 0)
        {
            throw new DbUpdateException(ErrorMessagesConstants.FailedToCreateEntity(typeof(FaqQuestion)));
        }

        scope.Complete();
        FaqQuestionDto createdEntity = _mapper.Map<FaqQuestionDto>(entity);
        return createdEntity;
    }
}
