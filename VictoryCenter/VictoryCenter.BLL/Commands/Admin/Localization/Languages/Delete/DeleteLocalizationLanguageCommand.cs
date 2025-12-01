using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.Localization.Languages.Delete;

public record DeleteLocalizationLanguageCommand(long Id) : IRequest<Result<long>>;
