using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs;

namespace VictoryCenter.BLL.Commands.Pages.CreatePage;

public record CreatePageCommand(CreatePageDto createPageDto)
    : IRequest<Result<PageDto>>;
