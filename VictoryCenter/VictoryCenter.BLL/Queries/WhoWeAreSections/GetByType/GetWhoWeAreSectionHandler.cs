using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.WhoWeAreSection;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.WhoWeAreSections.GetByType;

public class GetWhoWeAreSectionHandler : IRequestHandler<GetWhoWeAreSectionQuery, Result<WhoWeAreSectionDto>>
{
    private readonly IRepositoryWrapper _repository;
    private readonly IMapper _mapper;

    public GetWhoWeAreSectionHandler(IRepositoryWrapper repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<WhoWeAreSectionDto>> Handle(GetWhoWeAreSectionQuery request, CancellationToken cancellationToken)
    {
        if (Enum.TryParse<SectionType>(request.SectionType, out var parsedSectionType))
        {
            var section = await _repository.WhoWeAreSectionsRepository.GetFirstOrDefaultAsync(
                new QueryOptions<WhoWeAreSection>
                {
                    Filter = s => s.SectionType == parsedSectionType,
                    Include = s => s
                        .Include(sec => sec.Contents)
                        .ThenInclude(c => (c as ImageContent)!.Image)
                        .Include(sec => sec.Contents)
                        .ThenInclude(c => (c as CardContent)!.Image)!
                });
            return Result.Ok(_mapper.Map<WhoWeAreSectionDto>(section));
        }
        else
        {
            return Result.Fail("fafasf");
        }
    }
}
