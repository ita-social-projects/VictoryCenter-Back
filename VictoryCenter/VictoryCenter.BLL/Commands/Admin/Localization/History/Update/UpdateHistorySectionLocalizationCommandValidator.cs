using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Update;

namespace VictoryCenter.BLL.Commands.Admin.Localization.History.Update;

public class UpdateHistorySectionLocalizationCommandValidator : AbstractValidator<UpdateHistorySectionLocalizationCommand>
{
    public UpdateHistorySectionLocalizationCommandValidator(VictoryCenter.BLL.Validators.Localization.History.BaseHistorySectionContentLocalizationValidator contentValidator)
    {
        RuleFor(x => x.LanguageId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(UpdateHistorySectionLocalizationCommand.LanguageId)));

        RuleFor(x => x.UpdateDto)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateHistorySectionLocalizationCommand.UpdateDto)));

        RuleFor(x => x.UpdateDto.Contents)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateHistorySectionLocalizationDto.Contents)))
            .When(x => x.UpdateDto != null);

        RuleFor(x => x.UpdateDto.Contents)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(
                nameof(UpdateHistorySectionLocalizationDto.Contents)))
            .When(x => x.UpdateDto != null && x.UpdateDto.Contents != null);

        RuleFor(x => x.UpdateDto.Contents)
            .Must(list => list.All(item => item != null))
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainNullElements(
                nameof(UpdateHistorySectionLocalizationDto.Contents)))
            .When(x => x.UpdateDto != null && x.UpdateDto.Contents != null);

        RuleForEach(x => x.UpdateDto.Contents)
            .SetValidator(contentValidator)
            .When(x => x.UpdateDto != null && x.UpdateDto.Contents != null && x.UpdateDto.Contents.Any());
    }
}
