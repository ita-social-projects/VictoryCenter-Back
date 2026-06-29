using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.History.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Update;

namespace VictoryCenter.BLL.Validators.Localization.History;

public class UpdateHistorySectionLocalizationValidator : AbstractValidator<UpdateHistoryLocalizationCommand>
{
    public UpdateHistorySectionLocalizationValidator(BaseHistorySectionContentLocalizationValidator contentValidator)
    {
        RuleFor(x => x.LanguageId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(UpdateHistoryLocalizationCommand.LanguageId)));

        RuleFor(x => x.UpdateHistorySectionLocalizationDtos)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateHistoryLocalizationCommand.UpdateHistorySectionLocalizationDtos)));

        RuleFor(x => x.UpdateHistorySectionLocalizationDtos)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(
                nameof(UpdateHistoryLocalizationCommand.UpdateHistorySectionLocalizationDtos)))
            .When(x => x.UpdateHistorySectionLocalizationDtos != null);

        RuleFor(x => x.UpdateHistorySectionLocalizationDtos)
            .Must(list => list.All(item => item != null))
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainNullElements(
                nameof(UpdateHistoryLocalizationCommand.UpdateHistorySectionLocalizationDtos)))
            .When(x => x.UpdateHistorySectionLocalizationDtos != null);

        RuleForEach(x => x.UpdateHistorySectionLocalizationDtos)
            .ChildRules(section =>
            {
                section.RuleFor(s => s.Contents)
                    .NotNull()
                    .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                        nameof(UpdateHistorySectionLocalizationDto.Contents)));

                section.RuleFor(s => s.Contents)
                    .NotEmpty()
                    .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(
                        nameof(UpdateHistorySectionLocalizationDto.Contents)))
                    .When(s => s.Contents != null);

                section.RuleFor(s => s.Contents)
                    .Must(list => list.All(item => item != null))
                    .WithMessage(ErrorMessagesConstants.CollectionCannotContainNullElements(
                        nameof(UpdateHistorySectionLocalizationDto.Contents)))
                    .When(s => s.Contents != null);

                section.RuleForEach(s => s.Contents)
                    .SetValidator(contentValidator)
                    .When(s => s.Contents != null && s.Contents.Any());
            })
            .When(x => x.UpdateHistorySectionLocalizationDtos != null && x.UpdateHistorySectionLocalizationDtos.Any());
    }
}
