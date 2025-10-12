using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.Languages;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Commands.Admin.Localization.Languages.Create;

public record CreateLocalizationLanguageCommand(CreateLocalizationLanguageDto CreateLocalizationLanguageDto)
    : IRequest<Result<LocalizationLanguageDto>>;
