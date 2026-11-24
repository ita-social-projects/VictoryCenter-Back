using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;

namespace VictoryCenter.BLL.Queries.Public.Localization.HippotherapyPrograms.GetByEntityId;

public record GetHippotherapyProgramLocalizationByEntityIdQuery(long Id)
    : IRequest<Result<IEnumerable<HippotherapyProgramLocalizationDto>>>;
