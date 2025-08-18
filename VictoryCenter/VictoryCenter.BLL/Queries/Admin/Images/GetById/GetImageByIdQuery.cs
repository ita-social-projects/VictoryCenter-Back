using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Images;

namespace VictoryCenter.BLL.Queries.Admin.Images.GetById;

public record GetImageByIdQuery(long Id) : IRequest<Result<ImageDto>>;
