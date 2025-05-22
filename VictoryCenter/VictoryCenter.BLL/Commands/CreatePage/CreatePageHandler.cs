using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs;

namespace VictoryCenter.BLL.Commands.CreatePage;

public class CreatePageHandler : IRequestHandler<CreatePageCommand, Result<PageDto>>
{
    public Task<Result<PageDto>> Handle(CreatePageCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
