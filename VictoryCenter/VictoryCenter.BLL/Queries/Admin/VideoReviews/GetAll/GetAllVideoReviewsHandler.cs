using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.VideoReviews.GetAll;

public class GetAllVideoReviewsHandler : IRequestHandler<GetAllVideoReviewsQuery, Result<List<VideoReviewDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetAllVideoReviewsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<VideoReviewDto>>> Handle(
        GetAllVideoReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var videoReviews = await _repositoryWrapper.VideoReviewsRepository.GetAllAsync(
            new QueryOptions<VideoReview>
            {
                OrderByASC = videoReview => videoReview.Priority,
                AsNoTracking = true
            });

        return Result.Ok(_mapper.Map<List<VideoReviewDto>>(videoReviews));
    }
}
