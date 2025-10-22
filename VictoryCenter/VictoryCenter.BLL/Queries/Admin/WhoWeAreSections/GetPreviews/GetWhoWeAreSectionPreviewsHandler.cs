using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Queries.Admin.WhoWeAreSections.GetPreviews;

public class GetWhoWeAreSectionPreviewsHandler : IRequestHandler<GetWhoWeAreSectionPreviewsQuery, Result<List<WhoWeAreSectionInfoDto>>>
{
    private readonly IRepositoryWrapper _repository;
    private readonly IMapper _mapper;

    public GetWhoWeAreSectionPreviewsHandler(IRepositoryWrapper repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<List<WhoWeAreSectionInfoDto>>> Handle(GetWhoWeAreSectionPreviewsQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.WhoWeAreSectionsRepository.GetAllAsync();
        return Result.Ok(_mapper.Map<List<WhoWeAreSectionInfoDto>>(result));
    }
}
