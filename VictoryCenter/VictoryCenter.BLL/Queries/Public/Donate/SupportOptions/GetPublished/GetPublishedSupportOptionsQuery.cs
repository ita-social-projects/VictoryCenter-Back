using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.Donate.SupportOptions;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Queries.Public.Donate.SupportOptions.GetPublished;
public record GetPublishedSupportOptionsQuery(BankCurrency Currency) : IRequest<Result<List<SupportOptionsDto>>>;
