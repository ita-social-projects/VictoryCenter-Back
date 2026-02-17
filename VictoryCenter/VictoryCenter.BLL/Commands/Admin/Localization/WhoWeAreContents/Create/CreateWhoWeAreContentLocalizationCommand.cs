using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Commands.Admin.Localization.WhoWeAreContents.Create;

public record CreateWhoWeAreContentLocalizationCommand(SectionType SectionType, List<CreateWhoWeAreContentLocalizationDto> ContentLocalizationDtos)
    : IRequest<Result<List<WhoWeAreContentLocalizationDto>>>;
