using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Commands.Admin.Images.Update;

public record UpdateImageCommand(UpdateImageDto UpdateImageDto, long Id)
    : IRequest<Result<ImageDto>>;
