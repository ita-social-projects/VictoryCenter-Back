#pragma warning disable IDE0005
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Public.WhoWeArePage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Public.WhoWeAre.GetWhoWeArePage;

public class GetWhoWeArePageHandler : IRequestHandler<GetWhoWeArePageQuery, Result<List<WhoWeArePageSectionDto>>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public GetWhoWeArePageHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<List<WhoWeArePageSectionDto>>> Handle(GetWhoWeArePageQuery request, CancellationToken cancellationToken)
    {
        var result = await _repositoryWrapper.WhoWeAreSectionsRepository.GetAllAsync(new QueryOptions<WhoWeAreSection>
        {
            Include = query => query
                .Include(section => section.Contents)
                .ThenInclude(content => (content as CardContent)!.Image)
                .Include(section => section.Contents)
                .ThenInclude(content => (content as ImageContent)!.Image)!,
        });

        return Result.Ok(_mapper.Map<List<WhoWeArePageSectionDto>>(result));
    }
}
