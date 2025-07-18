using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Pages;

namespace VictoryCenter.BLL.Queries.Pages.GetAll;

public record GetAllPagesQuery
    : IRequest<Result<List<PageDto>>>;
