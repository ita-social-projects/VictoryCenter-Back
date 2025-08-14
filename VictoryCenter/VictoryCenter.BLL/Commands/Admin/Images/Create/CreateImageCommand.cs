using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Images;

namespace VictoryCenter.BLL.Commands.Admin.Images.Create;

public record CreateImageCommand(CreateImageDto CreateImageDto) : IRequest<Result<ImageDto>>;
