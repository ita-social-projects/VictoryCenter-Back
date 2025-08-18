using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.VisitorPages;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Queries.Admin.VisitorPages.GetAll;

public class GetAllVisitorPagesHandler : IRequestHandler<GetAllVisitorPagesQuery, Result<List<VisitorPageDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetAllVisitorPagesHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<VisitorPageDto>>> Handle(
        GetAllVisitorPagesQuery request,
        CancellationToken cancellationToken)
    {
        var entities = await _repositoryWrapper.VisitorPagesRepository.GetAllAsync();
        return Result.Ok(_mapper.Map<List<VisitorPageDto>>(entities));
    }
}
