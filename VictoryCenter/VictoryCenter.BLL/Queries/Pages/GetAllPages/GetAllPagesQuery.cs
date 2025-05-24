using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs;

namespace VictoryCenter.BLL.Queries.Pages.GetAllPages;

public record GetAllPagesQuery()
    : IRequest<Result<GetAllPagesDto>>;
