using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.VisitorPages;

namespace VictoryCenter.BLL.Queries.Admin.VisitorPages.GetAll;

public record GetAllVisitorPagesQuery
    : IRequest<Result<List<VisitorPageDto>>>;
