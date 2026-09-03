using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.VideoReviews.Create;

public class CreateVideoReviewHandler : IRequestHandler<CreateVideoReviewCommand, Result<VideoReviewDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly TimeProvider _timeProvider;
    private readonly IReorderService _reorderService;

    public CreateVideoReviewHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        TimeProvider timeProvider,
        IReorderService reorderService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _timeProvider = timeProvider;
        _reorderService = reorderService;
    }

    public async Task<Result<VideoReviewDto>> Handle(
        CreateVideoReviewCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedDto = request.VideoReview with
        {
            Title = request.VideoReview.Title.Trim(),
            Link = request.VideoReview.Link.Trim()
        };

        var entity = _mapper.Map<VideoReview>(normalizedDto);
        entity.CreatedAt = _timeProvider.GetUtcNow();
        entity.Priority = await _reorderService.GetNextDisplayOrderAsync<VideoReview>();

        await _repositoryWrapper.VideoReviewsRepository.CreateAsync(entity);

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
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(VideoReview)));
        }

        return Result.Fail<VideoReviewDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(VideoReview)));
    }
}
