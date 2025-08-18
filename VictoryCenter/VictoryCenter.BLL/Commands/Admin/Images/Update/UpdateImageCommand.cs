using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Images;

namespace VictoryCenter.BLL.Commands.Admin.Images.Update;

public record UpdateImageCommand(UpdateImageDto UpdateImageDto, long Id)
    : IRequest<Result<ImageDto>>;
