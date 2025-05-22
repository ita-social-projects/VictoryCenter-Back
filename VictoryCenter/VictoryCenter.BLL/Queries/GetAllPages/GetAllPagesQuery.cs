using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs;

namespace VictoryCenter.BLL.Queries.GetAllPages;

public record GetAllPagesQuery()
    : IRequest<Result<GetAllPagesDto>>;
