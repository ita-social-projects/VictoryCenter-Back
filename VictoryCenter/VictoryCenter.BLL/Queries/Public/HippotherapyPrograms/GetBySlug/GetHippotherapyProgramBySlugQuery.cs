using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;

namespace VictoryCenter.BLL.Queries.Public.HippotherapyPrograms.GetBySlug;

public record GetHippotherapyProgramBySlugQuery(string Slug) : IRequest<Result<HippotherapyProgramDto>>;
