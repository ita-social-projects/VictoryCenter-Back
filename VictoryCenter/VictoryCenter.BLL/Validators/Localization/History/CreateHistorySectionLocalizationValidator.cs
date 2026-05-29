using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.History.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Create;

namespace VictoryCenter.BLL.Validators.Localization.History;

public class CreateHistorySectionLocalizationValidator : AbstractValidator<CreateHistoryLocalizationCommand>
{
    public CreateHistorySectionLocalizationValidator(BaseHistorySectionContentLocalizationValidator contentValidator)
    {
        RuleFor(x => x.CreateHistorySectionLocalizationDtos)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateHistoryLocalizationCommand.CreateHistorySectionLocalizationDtos)));

        RuleFor(x => x.CreateHistorySectionLocalizationDtos)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(
                nameof(CreateHistoryLocalizationCommand.CreateHistorySectionLocalizationDtos)))
            .When(x => x.CreateHistorySectionLocalizationDtos != null);

        RuleFor(x => x.CreateHistorySectionLocalizationDtos)
            .Must(list => list.All(item => item != null))
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainNullElements(
                nameof(CreateHistoryLocalizationCommand.CreateHistorySectionLocalizationDtos)))
            .When(x => x.CreateHistorySectionLocalizationDtos != null);

        RuleForEach(x => x.CreateHistorySectionLocalizationDtos)
            .ChildRules(section =>
            {
                section.RuleFor(s => s.Contents)
                    .NotNull()
                    .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                        nameof(CreateHistorySectionLocalizationDto.Contents)));

                section.RuleFor(s => s.Contents)
                    .NotEmpty()
                    .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(
                        nameof(CreateHistorySectionLocalizationDto.Contents)))
                    .When(s => s.Contents != null);

                section.RuleFor(s => s.Contents)
                    .Must(list => list.All(item => item != null))
                    .WithMessage(ErrorMessagesConstants.CollectionCannotContainNullElements(
                        nameof(CreateHistorySectionLocalizationDto.Contents)))
                    .When(s => s.Contents != null);

                section.RuleForEach(s => s.Contents)
                    .SetValidator(contentValidator)
                    .When(s => s.Contents != null && s.Contents.Any());
            })
            .When(x => x.CreateHistorySectionLocalizationDtos != null && x.CreateHistorySectionLocalizationDtos.Any());
    }
}
