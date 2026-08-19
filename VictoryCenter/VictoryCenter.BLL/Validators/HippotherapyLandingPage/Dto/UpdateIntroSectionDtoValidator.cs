using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.HippotherapyLandingPage.Dto;

public class UpdateIntroSectionDtoValidator : AbstractValidator<UpdateIntroSectionDto>
{
    public UpdateIntroSectionDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateIntroSectionDto.Title)))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length >= HippotherapyLandingPageConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateIntroSectionDto.Title), HippotherapyLandingPageConstants.TitleMinLength))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length <= HippotherapyLandingPageConstants.IntroTitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateIntroSectionDto.Title), HippotherapyLandingPageConstants.IntroTitleMaxLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateIntroSectionDto.Description)))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length >= HippotherapyLandingPageConstants.TextMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateIntroSectionDto.Description), HippotherapyLandingPageConstants.TextMinLength))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length <= HippotherapyLandingPageConstants.IntroDescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateIntroSectionDto.Description), HippotherapyLandingPageConstants.IntroDescriptionMaxLength));

        RuleFor(x => x.ImageId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateIntroSectionDto.ImageId)))
            .When(x => x.ImageId.HasValue);
    }
}
