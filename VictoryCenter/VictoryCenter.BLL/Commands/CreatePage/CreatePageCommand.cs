using MediatR;
using FluentResults;
using VictoryCenter.BLL.DTOs;

namespace VictoryCenter.BLL.Commands.CreatePage;

public record CreatePageCommand(CreatePageDto createPageDto)
    : IRequest<Result<PageDto>>;
