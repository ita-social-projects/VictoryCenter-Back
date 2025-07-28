using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Images;

namespace VictoryCenter.BLL.Queries.Images.GetByName;

public record GetImageByNameQuery(string Name) : IRequest<Result<ImageDTO>>;
