using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.FeedbackReviews.GetById;

public class GetFeedbackReviewByIdHandler : IRequestHandler<GetFeedbackReviewByIdQuery, Result<FeedbackReviewDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetFeedbackReviewByIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<FeedbackReviewDto>> Handle(
        GetFeedbackReviewByIdQuery request,
        CancellationToken cancellationToken)
    {
        var review = await _repositoryWrapper.FeedbackReviewsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<FeedbackReview>
            {
                Filter = entity => entity.Id == request.Id,
                AsNoTracking = true
            });

        return review is null
            ? Result.Fail<FeedbackReviewDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(FeedbackReview)))
            : Result.Ok(_mapper.Map<FeedbackReviewDto>(review));
    }
}
