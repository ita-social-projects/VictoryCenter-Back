using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.VideoReviews.Update;

public class UpdateVideoReviewHandler : IRequestHandler<UpdateVideoReviewCommand, Result<VideoReviewDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public UpdateVideoReviewHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<VideoReviewDto>> Handle(
        UpdateVideoReviewCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await _repositoryWrapper.VideoReviewsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<VideoReview>
            {
                Filter = videoReview => videoReview.Id == request.Id,
                AsNoTracking = false
            });

        if (entity is null)
        {
            return Result.Fail<VideoReviewDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(VideoReview)));
        }

        var normalizedDto = request.VideoReview with
        {
            Title = request.VideoReview.Title.Trim(),
            Link = request.VideoReview.Link.Trim()
        };

        if (string.Equals(entity.Title, normalizedDto.Title, StringComparison.Ordinal) &&
            string.Equals(entity.Link, normalizedDto.Link, StringComparison.Ordinal) &&
            entity.Status == normalizedDto.Status)
        {
            return Result.Ok(_mapper.Map<VideoReviewDto>(entity));
        }

        _mapper.Map(normalizedDto, entity);

        try
        {
            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(_mapper.Map<VideoReviewDto>(entity));
            }
        }
        catch (DbUpdateException)
        {
            return Result.Fail<VideoReviewDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(VideoReview)));
        }

        return Result.Fail<VideoReviewDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(VideoReview)));
    }
}
