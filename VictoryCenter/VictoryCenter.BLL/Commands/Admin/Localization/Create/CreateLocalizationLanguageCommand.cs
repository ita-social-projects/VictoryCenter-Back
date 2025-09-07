using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Commands.Admin.Localization.Create;

public record CreateLocalizationLanguageCommand(CreateLocalizationLanguageDto CreateLocalizationLanguageDto)
    : IRequest<Result<LocalizationLanguageDto>>;
