using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.WhoWeAreSection;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Queries.WhoWeAreSections.GetByType;

public record GetWhoWeAreSectionQuery(SectionType SectionType) : IRequest<Result<WhoWeAreSectionDto>>;
