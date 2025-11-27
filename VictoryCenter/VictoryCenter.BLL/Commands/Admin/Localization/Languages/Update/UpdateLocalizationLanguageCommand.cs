using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.Languages;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Commands.Admin.Localization.Languages.Update;

public record UpdateLocalizationLanguageCommand(UpdateLocalizationLanguageDto UpdateLocalizationLanguageDto, long Id)
    : IRequest<Result<LocalizationLanguageDto>>;
