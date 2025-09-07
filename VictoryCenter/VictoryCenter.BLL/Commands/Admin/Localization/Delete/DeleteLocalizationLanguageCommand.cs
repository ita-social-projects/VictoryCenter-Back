using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.Localization.Delete;

public record DeleteLocalizationLanguageCommand(long Id) : IRequest<Result<long>>;
