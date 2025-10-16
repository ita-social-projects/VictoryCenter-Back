using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.HippotherapyPrograms;

namespace VictoryCenter.BLL.Queries.Public.HippotherapyPrograms.GetPublished;

public record GetPublishedProgramsQuery : IRequest<Result<List<PublishedHippotherapyProgramDto>>>;
