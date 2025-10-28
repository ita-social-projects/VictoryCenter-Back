using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Queries.Admin.Images.GetByName;

public record GetImageByNameQuery(string Name) : IRequest<Result<ImageDto>>;
