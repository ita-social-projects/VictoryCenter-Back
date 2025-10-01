using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Queries.Admin.Partners.GetAll;

public record GetAllPartnersSectionsQuery() : IRequest<Result<IEnumerable<PartnersSectionDto>>>;
