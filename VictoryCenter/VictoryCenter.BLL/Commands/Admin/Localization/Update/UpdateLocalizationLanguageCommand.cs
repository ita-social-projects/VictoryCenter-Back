using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Commands.Admin.Localization.Update;

public record UpdateLocalizationLanguageCommand(UpdateLocalizationLanguageDto UpdateLocalizationLanguageDto, long Id)
    : IRequest<Result<LocalizationLanguageDto>>;
