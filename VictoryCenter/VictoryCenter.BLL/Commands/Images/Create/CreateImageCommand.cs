using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Images;

namespace VictoryCenter.BLL.Commands.Images.Create;

public record CreateImageCommand(CreateImageDTO CreateImageDto) : IRequest<Result<ImageDTO>>;
