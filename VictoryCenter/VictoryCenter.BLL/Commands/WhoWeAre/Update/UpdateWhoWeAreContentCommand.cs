using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.WhoWeAreContent;
using VictoryCenter.BLL.DTOs.WhoWeAreSection;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Commands.WhoWeAre.Update;

public record UpdateWhoWeAreContentCommand(SectionType SectionType, List<CreateWhoWeAreContentDto> Content) : IRequest<Result<WhoWeAreSectionDto>>;
