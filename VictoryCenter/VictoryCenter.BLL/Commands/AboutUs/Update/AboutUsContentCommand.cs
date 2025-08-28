using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.AboutUsContent;

namespace VictoryCenter.BLL.Commands.AboutUs.Update;

public record AboutUsContentCommand(long SectionId, List<CreateAboutUsContentDto> Content) : IRequest<Result<AboutUsSectionDto>>;
