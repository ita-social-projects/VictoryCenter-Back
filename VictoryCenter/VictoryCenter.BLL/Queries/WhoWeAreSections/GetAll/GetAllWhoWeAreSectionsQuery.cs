using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.WhoWeAreSection;

namespace VictoryCenter.BLL.Queries.WhoWeAreSections.GetAll;

public record GetAllWhoWeAreSectionsQuery : IRequest<Result<List<WhoWeAreSectionInfoDto>>>
{
}
