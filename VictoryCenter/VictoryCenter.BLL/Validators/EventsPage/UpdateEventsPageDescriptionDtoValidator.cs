using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventsPage;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.EventsPage;

public class UpdateEventsPageDescriptionDtoValidator : AbstractValidator<UpdateEventsPageDescriptionDto>
{
    public UpdateEventsPageDescriptionDtoValidator()
    {
        RuleFor(x => x.PageDescription)
            .Must(value => !string.IsNullOrWhiteSpace(HtmlContentHelper.GetVisibleText(value)))
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateEventsPageDescriptionDto.PageDescription)))
            .Must(value => HtmlContentHelper.GetVisibleText(value).Length >= EventsPageConstants.PageDescriptionMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateEventsPageDescriptionDto.PageDescription), EventsPageConstants.PageDescriptionMinLength))
            .Must(value => HtmlContentHelper.GetVisibleText(value).Length <= EventsPageConstants.PageDescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateEventsPageDescriptionDto.PageDescription), EventsPageConstants.PageDescriptionMaxLength));
    }
}
