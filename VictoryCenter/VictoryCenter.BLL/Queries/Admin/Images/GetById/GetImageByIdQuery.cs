using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Queries.Admin.Images.GetById;

public record GetImageByIdQuery(long Id) : IRequest<Result<ImageDto>>;
