using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Queries.Admin.WhoWeAreSections.GetByType;

public record GetWhoWeAreSectionQuery(SectionType SectionType) : IRequest<Result<WhoWeAreSectionDto>>;
