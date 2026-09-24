using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventsPage;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.EventsPage;

public class UpdateEventsBlockTitleDtoValidator : AbstractValidator<UpdateEventsBlockTitleDto>
{
    public UpdateEventsBlockTitleDtoValidator()
    {
        RuleFor(x => x.EventsBlockTitle)
            .Must(value => !string.IsNullOrWhiteSpace(HtmlContentHelper.StripHtmlTags(value)))
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateEventsBlockTitleDto.EventsBlockTitle)))
            .Must(value => HtmlContentHelper.StripHtmlTags(value).Trim().Length >= EventsPageConstants.EventsBlockTitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateEventsBlockTitleDto.EventsBlockTitle), EventsPageConstants.EventsBlockTitleMinLength))
            .Must(value => HtmlContentHelper.StripHtmlTags(value).Trim().Length <= EventsPageConstants.EventsBlockTitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateEventsBlockTitleDto.EventsBlockTitle), EventsPageConstants.EventsBlockTitleMaxLength));
    }
}
