using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyPrograms.GetByLanguageId;

public record GetHippotherapyProgramLocalizationByLanguageIdQuery(long Id)
    : IRequest<Result<List<HippotherapyProgramLocalizationDto>>>;
