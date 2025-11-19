using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;

namespace VictoryCenter.BLL.Validators.Localization.TeamMembers;

public class CreateTeamMemberLocalizationValidator : AbstractValidator<CreateTeamMemberLocalizationCommand>
{
    public CreateTeamMemberLocalizationValidator(BaseTeamMemberLocalizationValidator baseTeamMemberLocalizationsValidator)
    {
        RuleFor(c => c.CreateTeamMemberLocalizationDto).SetValidator(baseTeamMemberLocalizationsValidator);

        RuleFor(x => x.CreateTeamMemberLocalizationDto.EntityId)
            .NotNull().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateTeamMemberLocalizationDto.EntityId)))
            .GreaterThan(0).WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateTeamMemberLocalizationDto.EntityId)));
        RuleFor(x => x.CreateTeamMemberLocalizationDto.LanguageId)
          .NotNull().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateTeamMemberLocalizationDto.LanguageId)))
          .GreaterThan(0).WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateTeamMemberLocalizationDto.LanguageId)));
    }
}
