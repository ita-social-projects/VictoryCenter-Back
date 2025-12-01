using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Queries.Admin.Partners.GetPartnerSections;

public record GetPartnerSectionsQuery() : IRequest<Result<IEnumerable<PartnersSectionDto>>>;
