using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ReportMediaSettings.UpdateReportMediaSettings;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportMediaSettings;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.ReportMediaSettings.Commands;

public class UpdateReportMediaSettingsCommandValidator : AbstractValidator<UpdateReportMediaSettingsCommand>
{
    public UpdateReportMediaSettingsCommandValidator()
    {
        RuleFor(x => x.Dto.ChangedLivesBlock.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateChangedLivesBlockDto.Title)))
            .MinimumLength(ChangedLivesBlockConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateChangedLivesBlockDto.Title), ChangedLivesBlockConstants.TitleMinLength))
            .MaximumLength(ChangedLivesBlockConstants.TitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateChangedLivesBlockDto.Title), ChangedLivesBlockConstants.TitleMaxLength));

        RuleFor(x => x.Dto.ChangedLivesBlock.ChangedLives)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateChangedLivesBlockDto.ChangedLives)))
            .GreaterThanOrEqualTo(ChangedLivesBlockConstants.ChangeLivesMinDigits)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThanOrEqualToN(
                nameof(UpdateChangedLivesBlockDto.ChangedLives), ChangedLivesBlockConstants.ChangeLivesMinDigits))
            .Must((command, value) => ValidationHelpers.HaveMaximumDigitsInt(ChangedLivesBlockConstants.ChangeLivesMaxDigits)(value))
            .WithMessage(ErrorMessagesConstants.PropertyMustBeLessThanOrEqualToN(
                nameof(UpdateChangedLivesBlockDto.ChangedLives), ChangedLivesBlockConstants.ChangeLivesMaxDigits));

        RuleFor(x => x.Dto.ChangedLivesBlock.ImageId)
            .GreaterThan(0).WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateChangedLivesBlockDto.ImageId)));

        RuleFor(x => x.Dto.CollectedFundsBlock.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateCollectedFundsBlockDto.Title)))
            .MinimumLength(CollectedFundsBlockConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateCollectedFundsBlockDto.Title), CollectedFundsBlockConstants.TitleMinLength))
            .MaximumLength(CollectedFundsBlockConstants.TitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateCollectedFundsBlockDto.Title), CollectedFundsBlockConstants.TitleMaxLength));

        RuleFor(x => x.Dto.CollectedFundsBlock.CollectedFunds)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateCollectedFundsBlockDto.CollectedFunds)))
            .GreaterThanOrEqualTo(CollectedFundsBlockConstants.CollectedAmountMinDigits)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThanOrEqualToN(
                nameof(UpdateCollectedFundsBlockDto.CollectedFunds), CollectedFundsBlockConstants.CollectedAmountMinDigits))
            .Must((command, value) => ValidationHelpers.HaveMaximumDigits(CollectedFundsBlockConstants.CollectedAmountMaxDigits)(value))
            .WithMessage(ErrorMessagesConstants.PropertyMustBeLessThanOrEqualToN(
                nameof(UpdateCollectedFundsBlockDto.CollectedFunds), CollectedFundsBlockConstants.CollectedAmountMaxDigits));

        RuleFor(x => x.Dto.CollectedFundsBlock.ImageId)
            .GreaterThan(0).WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateCollectedFundsBlockDto.ImageId)));
    }
}
