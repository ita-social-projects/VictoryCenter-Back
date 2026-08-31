using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.FeedbackReviews.Create;

public class CreateFeedbackReviewHandler : IRequestHandler<CreateFeedbackReviewCommand, Result<FeedbackReviewDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IReorderService _reorderService;
    private readonly TimeProvider _timeProvider;

    public CreateFeedbackReviewHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IReorderService reorderService,
        TimeProvider timeProvider)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _reorderService = reorderService;
        _timeProvider = timeProvider;
    }

    public async Task<Result<FeedbackReviewDto>> Handle(
        CreateFeedbackReviewCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedDto = request.CreateFeedbackReviewDto with
        {
            AuthorName = request.CreateFeedbackReviewDto.AuthorName.Trim(),
            Text = request.CreateFeedbackReviewDto.Text.Trim()
        };

        var feedbackReview = _mapper.Map<FeedbackReview>(normalizedDto);
        feedbackReview.CreatedAt = _timeProvider.GetUtcNow();
        feedbackReview.Priority = await _reorderService.GetNextDisplayOrderAsync<FeedbackReview>();

        await _repositoryWrapper.FeedbackReviewsRepository.CreateAsync(feedbackReview);

        try
        {
            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(_mapper.Map<FeedbackReviewDto>(feedbackReview));
            }
        }
        catch (DbUpdateException)
        {
            return Result.Fail<FeedbackReviewDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(FeedbackReview)));
        }

        return Result.Fail<FeedbackReviewDto>(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(FeedbackReview)));
    }
}
