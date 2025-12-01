using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Validators.HippotherapyPrograms;

public class BaseHippotherapyProgramValidator : AbstractValidator<CreateHippotherapyProgramDto>
{
    public BaseHippotherapyProgramValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HippotherapyProgramDto.Name)))
            .MaximumLength(HippotherapyProgramConstants.MaxNameLength)
            .WithMessage(
                ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(HippotherapyProgramDto.Name), HippotherapyProgramConstants.MaxNameLength))
            .MinimumLength(HippotherapyProgramConstants.MinNameLength)
            .WithMessage(
                ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(HippotherapyProgramDto.Name), HippotherapyProgramConstants.MinNameLength));

        RuleFor(x => x.Description)
            .MaximumLength(HippotherapyProgramConstants.MaxDescriptionLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(HippotherapyProgramDto.Description), HippotherapyProgramConstants.MaxDescriptionLength))
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HippotherapyProgramDto.Description)))
            .MinimumLength(HippotherapyProgramConstants.MinDescriptionLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(HippotherapyProgramDto.Description), HippotherapyProgramConstants.MinDescriptionLength))
            .When(x => x.Status == Status.Published);

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage(ErrorMessagesConstants.UnknownStatusValue);

        RuleFor(x => x.CategoryIds)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HippotherapyProgramDto.Categories)))
            .Must(list => list.Distinct().Count() == list.Count)
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(CreateHippotherapyProgramDto.CategoryIds)));

        RuleFor(x => x.BackgroundImageId)
            .NotNull().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateHippotherapyProgramDto.BackgroundImageId)))
            .GreaterThan(0).WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateHippotherapyProgramDto.BackgroundImageId)))
            .When(x => x.Status == Status.Published);

        RuleFor(x => x.PreviewImageId)
           .NotNull().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateHippotherapyProgramDto.PreviewImageId)))
           .GreaterThan(0).WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateHippotherapyProgramDto.PreviewImageId)))
           .When(x => x.Status == Status.Published);

        RuleFor(x => x.Location)
            .MaximumLength(HippotherapyProgramConstants.MaxLocationLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(HippotherapyProgramDto.Location), HippotherapyProgramConstants.MaxLocationLength))
            .MinimumLength(HippotherapyProgramConstants.MinLocationLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(HippotherapyProgramDto.Location), HippotherapyProgramConstants.MinLocationLength))
            .When(x => !string.IsNullOrWhiteSpace(x.Location));

        RuleFor(x => x.ParticipantsCount)
            .MaximumLength(HippotherapyProgramConstants.MaxParticipantsCountLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(HippotherapyProgramDto.ParticipantsCount), HippotherapyProgramConstants.MaxParticipantsCountLength))
            .MinimumLength(HippotherapyProgramConstants.MinParticipantsCountLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(HippotherapyProgramDto.ParticipantsCount), HippotherapyProgramConstants.MinParticipantsCountLength))
            .When(x => !string.IsNullOrWhiteSpace(x.ParticipantsCount));

        RuleFor(x => x.MeetingsCount)
            .MaximumLength(HippotherapyProgramConstants.MaxMeetingsCountLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(HippotherapyProgramDto.MeetingsCount), HippotherapyProgramConstants.MaxMeetingsCountLength))
            .MinimumLength(HippotherapyProgramConstants.MinMeetingsCountLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(HippotherapyProgramDto.MeetingsCount), HippotherapyProgramConstants.MinMeetingsCountLength))
            .When(x => !string.IsNullOrWhiteSpace(x.MeetingsCount));
    }
}
