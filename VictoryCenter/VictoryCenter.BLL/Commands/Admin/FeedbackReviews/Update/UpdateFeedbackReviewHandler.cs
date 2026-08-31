using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.FeedbackReviews.Update;

public class UpdateFeedbackReviewHandler : IRequestHandler<UpdateFeedbackReviewCommand, Result<FeedbackReviewDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public UpdateFeedbackReviewHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<FeedbackReviewDto>> Handle(
        UpdateFeedbackReviewCommand request,
        CancellationToken cancellationToken)
    {
        var feedbackReview = await _repositoryWrapper.FeedbackReviewsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<FeedbackReview>
            {
                Filter = entity => entity.Id == request.Id,
                AsNoTracking = false
            });

        if (feedbackReview is null)
        {
            return Result.Fail<FeedbackReviewDto>(
                ErrorMessagesConstants.NotFound(request.Id, typeof(FeedbackReview)));
        }

        var normalizedDto = request.FeedbackReview with
        {
            AuthorName = request.FeedbackReview.AuthorName.Trim(),
            Text = request.FeedbackReview.Text.Trim()
        };

        feedbackReview.AuthorName = normalizedDto.AuthorName;
        feedbackReview.Text = normalizedDto.Text;
        feedbackReview.Status = normalizedDto.Status;

        _repositoryWrapper.FeedbackReviewsRepository.Update(feedbackReview);

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
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(FeedbackReview)));
        }

        return Result.Fail<FeedbackReviewDto>(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(FeedbackReview)));
    }
}
