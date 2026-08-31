using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.FeedbackHistories.GetAll;

public class GetAllFeedbackHistoriesHandler : IRequestHandler<GetAllFeedbackHistoriesQuery, Result<IEnumerable<FeedbackHistoryDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetAllFeedbackHistoriesHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<IEnumerable<FeedbackHistoryDto>>> Handle(
        GetAllFeedbackHistoriesQuery request, CancellationToken cancellationToken)
    {
        var entities = (await _repositoryWrapper.FeedbackHistoriesRepository.GetAllAsync(new QueryOptions<FeedbackHistory>
        {
            AsNoTracking = true,
            OrderByASC = e => e.Priority
        })).ToList();

        var imageIds = entities
            .Where(e => e.ImageId.HasValue)
            .Select(e => e.ImageId!.Value)
            .Distinct()
            .ToList();

        if (imageIds.Count > 0)
        {
            var images = await _repositoryWrapper.ImageRepository.GetAllAsync(new QueryOptions<Image>
            {
                AsNoTracking = true,
                Filter = img => imageIds.Contains(img.Id)
            });

            var imagesDict = images.ToDictionary(img => img.Id);

            foreach (var entity in entities)
            {
                if (entity.ImageId.HasValue && imagesDict.TryGetValue(entity.ImageId.Value, out var image))
                {
                    entity.Image = image;
                }
            }
        }

        return Result.Ok(_mapper.Map<IEnumerable<FeedbackHistoryDto>>(entities));
    }
}
