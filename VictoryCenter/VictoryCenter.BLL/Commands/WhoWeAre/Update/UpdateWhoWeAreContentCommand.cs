using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.WhoWeAreContent;
using VictoryCenter.BLL.DTOs.WhoWeAreSection;

namespace VictoryCenter.BLL.Commands.WhoWeAre.Update;

public record UpdateWhoWeAreContentCommand(long SectionId, List<CreateWhoWeAreContentDto> Content) : IRequest<Result<WhoWeAreSectionDto>>;
