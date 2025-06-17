using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Queries.Pages.GetAllPages;

public class GetAllPagesHandler : IRequestHandler<GetAllPagesQuery, Result<GetAllPagesDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetAllPagesHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public Task<Result<GetAllPagesDto>> Handle(GetAllPagesQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
