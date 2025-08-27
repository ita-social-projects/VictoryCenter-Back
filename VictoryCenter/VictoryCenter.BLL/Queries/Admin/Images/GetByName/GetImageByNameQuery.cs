using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Images;

namespace VictoryCenter.BLL.Queries.Admin.Images.GetByName;

public record GetImageByNameQuery(string Name) : IRequest<Result<ImageDto>>;
