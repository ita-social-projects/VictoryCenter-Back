using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.HypotherapyPrograms;

namespace VictoryCenter.BLL.Queries.Public.HypotherapyPrograms.GetPublished;

public record GetPublishedProgramsQuery : IRequest<Result<List<PublishedHypotherapyProgramDto>>>;
