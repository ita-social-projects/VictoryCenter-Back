using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Queries.Admin.WhoWeAreSections.GetAll;

public class GetAllWhoWeAreSectionsHandler : IRequestHandler<GetAllWhoWeAreSectionsQuery, Result<List<WhoWeAreSectionInfoDto>>>
{
    private readonly IRepositoryWrapper _repository;
    private readonly IMapper _mapper;

    public GetAllWhoWeAreSectionsHandler(IRepositoryWrapper repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<List<WhoWeAreSectionInfoDto>>> Handle(GetAllWhoWeAreSectionsQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.WhoWeAreSectionsRepository.GetAllAsync();
        return Result.Ok(_mapper.Map<List<WhoWeAreSectionInfoDto>>(result));
    }
}
