using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        var entities = await _repositoryWrapper.FeedbackHistoriesRepository.GetAllAsync(new QueryOptions<FeedbackHistory>
        {
            AsNoTracking = true,
            Include = q => q.Include(e => e.Image!)
        });
        return Result.Ok(_mapper.Map<IEnumerable<FeedbackHistoryDto>>(entities));
    }
}