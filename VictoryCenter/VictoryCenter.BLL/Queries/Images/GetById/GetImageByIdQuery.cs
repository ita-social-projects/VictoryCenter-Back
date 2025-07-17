using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Images;

namespace VictoryCenter.BLL.Queries.Images.GetById;

public record GetImageByIdQuery(long id) : IRequest<Result<ImageDTO>>;
