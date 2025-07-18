using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Pages;

namespace VictoryCenter.BLL.Queries.Admin.Pages.GetAll;

public record GetAllPagesQuery
    : IRequest<Result<List<PageDto>>>;
