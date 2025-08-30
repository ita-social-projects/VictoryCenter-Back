using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Programs;

namespace VictoryCenter.BLL.Queries.Programs.GetPublished;

public record GetPublishedProgramsQuery : IRequest<Result<List<PublishedProgramDto>>>;
