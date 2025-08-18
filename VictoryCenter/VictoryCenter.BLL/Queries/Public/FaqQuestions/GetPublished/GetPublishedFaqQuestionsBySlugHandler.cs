using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Public.FaqQuestions;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Public.FaqQuestions.GetPublished;

public class GetPublishedFaqQuestionsBySlugHandler
    : IRequestHandler<GetPublishedFaqQuestionsBySlugQuery, Result<List<PublishedFaqQuestionDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetPublishedFaqQuestionsBySlugHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<PublishedFaqQuestionDto>>> Handle(
        GetPublishedFaqQuestionsBySlugQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<FaqPlacement>
        {
            Include = placement => placement.Include(placement => placement.Question),
            Filter = placement => placement.Page.Slug == request.Slug && placement.Question.Status == Status.Published,
            OrderByASC = placement => placement.Priority
        };

        var publishedFaqFromVisitorPage = await _repositoryWrapper.FaqPlacementsRepository.GetAllAsync(queryOptions);
        if (!publishedFaqFromVisitorPage.Any())
        {
            return Result.Fail<List<PublishedFaqQuestionDto>>(FaqConstants.PageNotFoundOrContainsNoFaqQuestions);
        }

        var publishedFaqDtos = _mapper.Map<List<PublishedFaqQuestionDto>>(publishedFaqFromVisitorPage.Select(p => p.Question));
        return Result.Ok(publishedFaqDtos);
    }
}
