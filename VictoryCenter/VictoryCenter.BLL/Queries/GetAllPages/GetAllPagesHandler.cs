using FluentResults;
using VictoryCenter.BLL.DTOs;
using MediatR;

namespace VictoryCenter.BLL.Queries.GetAllPages;

public class GetAllPagesHandler : IRequestHandler<GetAllPagesQuery, Result<GetAllPagesDto>>
{
    public Task<Result<GetAllPagesDto>> Handle(GetAllPagesQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
