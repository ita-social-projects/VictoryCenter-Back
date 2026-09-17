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
            .Must(value => !string.IsNullOrWhiteSpace(HtmlContentHelper.StripHtmlTags(value)))
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateEventsPageDescriptionDto.PageDescription)))
            .Must(value => HtmlContentHelper.StripHtmlTags(value).Length <= 1000)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateEventsPageDescriptionDto.PageDescription), 1000));
    }
}
