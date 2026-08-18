using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.HippotherapyLandingPage.Dto;

public class UpdateTextSectionDtoValidator : AbstractValidator<UpdateTextSectionDto>
{
    public UpdateTextSectionDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateTextSectionDto.Title)))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length >= HippotherapyLandingPageConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateTextSectionDto.Title), HippotherapyLandingPageConstants.TitleMinLength))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length <= HippotherapyLandingPageConstants.TextSectionTitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateTextSectionDto.Title), HippotherapyLandingPageConstants.TextSectionTitleMaxLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateTextSectionDto.Description)))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length >= HippotherapyLandingPageConstants.TextMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateTextSectionDto.Description), HippotherapyLandingPageConstants.TextMinLength))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length <= HippotherapyLandingPageConstants.TextSectionDescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateTextSectionDto.Description), HippotherapyLandingPageConstants.TextSectionDescriptionMaxLength));
    }
}
