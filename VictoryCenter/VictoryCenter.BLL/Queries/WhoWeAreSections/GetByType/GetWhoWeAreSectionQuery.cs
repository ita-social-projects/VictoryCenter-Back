using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.WhoWeAreSection;

namespace VictoryCenter.BLL.Queries.WhoWeAreSections.GetByType;

public record GetWhoWeAreSectionQuery(string SectionType) : IRequest<Result<WhoWeAreSectionDto>>;
