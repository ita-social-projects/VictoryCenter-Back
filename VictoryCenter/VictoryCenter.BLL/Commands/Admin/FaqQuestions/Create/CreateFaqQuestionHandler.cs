using System.Transactions;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.FaqQuestions.Create;

public class CreateFaqQuestionHandler : BaseHandler<CreateFaqQuestionCommand, FaqQuestionDto>
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

    public override async Task<FaqQuestionDto> HandleRequest(CreateFaqQuestionCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var allPages = await _repositoryWrapper.VisitorPagesRepository.GetAllAsync();
        FaqQuestion entity = _mapper.Map<FaqQuestion>(request.CreateFaqQuestionDto);

        foreach (var pageId in request.CreateFaqQuestionDto.PageIds.Distinct())
        {
            if (!allPages.Any(p => p.Id == pageId))
            {
                throw new Exception(ErrorMessagesConstants.NotFound(pageId, typeof(VisitorPage)));
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
