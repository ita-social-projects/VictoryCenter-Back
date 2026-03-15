using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyPrograms;

public class BaseHippotherapyProgramLocalizationValidator : AbstractValidator<BaseHippotherapyProgramLocalizationDto>
{
    public BaseHippotherapyProgramLocalizationValidator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(HippotherapyProgramLocalizationConstants.NameMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateHippotherapyProgramLocalizationDto.Name),
                HippotherapyProgramLocalizationConstants.NameMinLength))
            .MaximumLength(HippotherapyProgramLocalizationConstants.NameMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateHippotherapyProgramLocalizationDto.Name),
                HippotherapyProgramLocalizationConstants.NameMaxLength));

        RuleFor(x => x.Description)
            .MinimumLength(HippotherapyProgramLocalizationConstants.DescriptionMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateHippotherapyProgramLocalizationDto.Description),
                HippotherapyProgramLocalizationConstants.DescriptionMinLength))
            .MaximumLength(HippotherapyProgramLocalizationConstants.DescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateHippotherapyProgramLocalizationDto.Description),
                HippotherapyProgramLocalizationConstants.DescriptionMaxLength));

        RuleFor(x => x.Location)
            .MinimumLength(HippotherapyProgramLocalizationConstants.LocationMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateHippotherapyProgramLocalizationDto.Location),
                HippotherapyProgramLocalizationConstants.LocationMinLength))
            .MaximumLength(HippotherapyProgramLocalizationConstants.LocationMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateHippotherapyProgramLocalizationDto.Location),
                HippotherapyProgramLocalizationConstants.LocationMaxLength));

        RuleFor(x => x.ParticipantsCount)
            .MinimumLength(HippotherapyProgramLocalizationConstants.ParticipantsCountMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateHippotherapyProgramLocalizationDto.ParticipantsCount),
                HippotherapyProgramLocalizationConstants.ParticipantsCountMinLength))
            .MaximumLength(HippotherapyProgramLocalizationConstants.ParticipantsCountMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateHippotherapyProgramLocalizationDto.ParticipantsCount),
                HippotherapyProgramLocalizationConstants.ParticipantsCountMaxLength));

        RuleFor(x => x.MeetingsCount)
            .MinimumLength(HippotherapyProgramLocalizationConstants.MeetingsCountMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateHippotherapyProgramLocalizationDto.MeetingsCount),
                HippotherapyProgramLocalizationConstants.MeetingsCountMinLength))
            .MaximumLength(HippotherapyProgramLocalizationConstants.MeetingsCountMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateHippotherapyProgramLocalizationDto.MeetingsCount),
                HippotherapyProgramLocalizationConstants.MeetingsCountMaxLength));
    }
}
