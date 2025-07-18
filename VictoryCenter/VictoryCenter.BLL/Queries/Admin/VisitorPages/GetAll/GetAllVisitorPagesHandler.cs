using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.VisitorPages;
using VictoryCenter.BLL.Queries.Admin.VisitorPages.GetAll;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Queries.Admin.Pages.GetAll;

public class GetAllPagesHandler : IRequestHandler<GetAllVisitorPagesQuery, Result<List<VisitorPageDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;

    public Task<Result<List<VisitorPageDto>>> Handle(GetAllVisitorPagesQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
