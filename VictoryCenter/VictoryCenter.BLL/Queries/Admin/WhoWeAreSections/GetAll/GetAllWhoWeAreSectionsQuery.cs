using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;

namespace VictoryCenter.BLL.Queries.Admin.WhoWeAreSections.GetAll;

public record GetAllWhoWeAreSectionsQuery : IRequest<Result<List<WhoWeAreSectionInfoDto>>>
{
}
