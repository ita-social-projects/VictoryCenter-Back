using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Images;

namespace VictoryCenter.BLL.Commands.Images.Update;

public record UpdateImageCommand(UpdateImageDTO updateImageDto)
    : IRequest<Result<ImageDTO>>;
