using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Pages;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Queries.Pages.GetAll;

public class GetAllPagesHandler : IRequestHandler<GetAllPagesQuery, Result<List<PageDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;

    public Task<Result<List<PageDto>>> Handle(GetAllPagesQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
