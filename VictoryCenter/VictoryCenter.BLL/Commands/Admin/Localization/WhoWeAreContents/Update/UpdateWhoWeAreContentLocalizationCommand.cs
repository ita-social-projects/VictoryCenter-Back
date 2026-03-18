using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Commands.Admin.Localization.WhoWeAreContents.Update;

public record UpdateWhoWeAreContentLocalizationCommand(SectionType SectionType, List<UpdateWhoWeAreContentLocalizationDto> ContentLocalizationDtos)
    : IRequest<Result<List<WhoWeAreContentLocalizationDto>>>;
