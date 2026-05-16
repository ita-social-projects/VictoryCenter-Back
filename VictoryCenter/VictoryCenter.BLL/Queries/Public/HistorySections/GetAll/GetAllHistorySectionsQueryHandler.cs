using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.HistorySection;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Public.HistorySections.GetAll;

public class GetAllHistorySectionsQueryHandler : IRequestHandler<GetAllHistorySectionsQuery, Result<List<HistorySectionDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetAllHistorySectionsQueryHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<HistorySectionDto>>> Handle(GetAllHistorySectionsQuery request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<HistorySection>
        {
            Include = section => section.Include(s => s.Contents).ThenInclude(c => c.Localizations).ThenInclude(l => l.Language),
            OrderByASC = section => section.Order
        };

        List<HistorySection> sections = (await _repositoryWrapper.HistorySectionsRepository.GetAllAsync(queryOptions)).ToList();

        foreach (var section in sections)
        {
            section.Contents = section.Contents
                .OrderBy(content => content.Order)
                .ToList();
        }

        var allImageIds = sections
            .SelectMany(s => s.Contents)
            .OfType<ImageHistoryContent>()
            .Select(c => c.ImageId)
            .Distinct();

        var imagesByIdResult = await ImageValidationHelper
            .ValidateAndGetImagesByIdsAsync(_repositoryWrapper, allImageIds);

        if (imagesByIdResult.IsFailed)
        {
            return Result.Fail<List<HistorySectionDto>>(imagesByIdResult.Errors);
        }

        var imagesById = imagesByIdResult.Value;

        foreach (var content in sections.SelectMany(s => s.Contents).OfType<ImageHistoryContent>()
                        .Where(c => c.ImageId > 0))
        {
            content.Image = imagesById[content.ImageId];
        }

        return Result.Ok(_mapper.Map<List<HistorySectionDto>>(sections));
    }
}
