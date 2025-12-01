using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Commands.Admin.Images.Create;

public record CreateImageCommand(CreateImageDto CreateImageDto) : IRequest<Result<ImageDto>>;
