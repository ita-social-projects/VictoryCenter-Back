using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreContent;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Commands.Admin.WhoWeAre.Update;

public record UpdateWhoWeAreContentCommand(SectionType SectionType, List<CreateWhoWeAreContentDto> Content) : IRequest<Result<WhoWeAreSectionDto>>;
