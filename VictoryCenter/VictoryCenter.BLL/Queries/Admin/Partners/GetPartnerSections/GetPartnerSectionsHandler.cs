using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Partners.GetPartnerSections;

public class GetPartnerSectionsHandler
    : IRequestHandler<GetPartnerSectionsQuery, Result<IEnumerable<PartnersSectionDto>>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public GetPartnerSectionsHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<PartnersSectionDto>>> Handle(
        GetPartnerSectionsQuery request,
        CancellationToken cancellationToken)
    {
        var sections = await _repositoryWrapper.PartnerSectionsRepository.GetAllAsync(
            new QueryOptions<PartnerSection>
            {
                Include = q => q.Include(s => s.Partners.OrderBy(p => p.Priority))
                                .ThenInclude(p => p.Image!),
                OrderByASC = s => s.Priority,
                AsNoTracking = true
            });

        var sectionDtos = _mapper.Map<IEnumerable<PartnersSectionDto>>(sections);

        return Result.Ok(sectionDtos);
    }
}
