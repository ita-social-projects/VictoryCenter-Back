using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.Programs;

namespace VictoryCenter.BLL.Queries.Public.Programs.GetPublished;

public record GetPublishedProgramsQuery : IRequest<Result<List<PublishedProgramDto>>>;
