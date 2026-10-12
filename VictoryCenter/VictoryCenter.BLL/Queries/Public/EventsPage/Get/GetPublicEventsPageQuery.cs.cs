using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.EventsPage;

namespace VictoryCenter.BLL.Queries.Public.EventsPage.Get;

public record GetPublicEventsPageQuery : IRequest<Result<PublicEventsPageDto>>;
