using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;

namespace VictoryCenter.BLL.Queries.Admin.Donate.SupportOptions.GetAll;
public record GetAllSupportOptionsQuery : IRequest<Result<List<SupportOptionsDto>>>;
