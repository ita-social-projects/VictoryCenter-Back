using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.WhoWeAreSections.GetByType;

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
        var section = await _repository.WhoWeAreSectionsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<WhoWeAreSection>
            {
                Filter = s => s.SectionType == request.SectionType,
                Include = s => s
                    .Include(sec => sec.Contents)
                    .ThenInclude(c => (c as ImageContent)!.Image)
                    .Include(sec => sec.Contents)
                    .ThenInclude(c => (c as CardContent)!.Image)!
                    .Include(sec => sec.Contents)
                    .ThenInclude(c => c.Localizations)
                    .ThenInclude(l => l.Language)
            });

        if (section == null)
        {
            return Result.Fail(ErrorMessagesConstants.NotFoundByIdentifier(request.SectionType, typeof(WhoWeAreSection)));
        }

        return Result.Ok(_mapper.Map<WhoWeAreSectionDto>(section));
    }
}
