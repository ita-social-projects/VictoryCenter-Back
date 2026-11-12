using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.HippotherapyLandingPage.Dto;

public class UpdateEthicsSectionDtoValidator : AbstractValidator<UpdateEthicsSectionDto>
{
    public UpdateEthicsSectionDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateEthicsSectionDto.Title)))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length >= HippotherapyLandingPageConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateEthicsSectionDto.Title), HippotherapyLandingPageConstants.TitleMinLength))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length <= HippotherapyLandingPageConstants.EthicsTitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateEthicsSectionDto.Title), HippotherapyLandingPageConstants.EthicsTitleMaxLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateEthicsSectionDto.Description)))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length >= HippotherapyLandingPageConstants.TextMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateEthicsSectionDto.Description), HippotherapyLandingPageConstants.TextMinLength))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length <= HippotherapyLandingPageConstants.EthicsDescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateEthicsSectionDto.Description), HippotherapyLandingPageConstants.EthicsDescriptionMaxLength));

        RuleFor(x => x.Principles)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateEthicsSectionDto.Principles)))
            .Must(p => p.Count == HippotherapyLandingPageConstants.EthicsPrinciplesCount)
            .WithMessage(ErrorMessagesConstants.CollectionMustContainExactlyNItems(
                nameof(UpdateEthicsSectionDto.Principles), HippotherapyLandingPageConstants.EthicsPrinciplesCount));

        RuleForEach(x => x.Principles)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateEthicsSectionDto.Principles)));

        RuleForEach(x => x.Principles).ChildRules(principle =>
        {
            principle.RuleFor(p => p.Id)
                .GreaterThan(0)
                .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateEthicsPrincipleDto.Id)))
                .When(p => p.Id.HasValue);

            principle.RuleFor(p => p.Text)
                .Must(text => HtmlContentHelper.StripHtmlTags(text ?? string.Empty).Length >= HippotherapyLandingPageConstants.TextMinLength)
                .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                    nameof(UpdateEthicsPrincipleDto.Text), HippotherapyLandingPageConstants.TextMinLength))
                .Must(text => HtmlContentHelper.StripHtmlTags(text ?? string.Empty).Length <= HippotherapyLandingPageConstants.EthicsPrincipleMaxLength)
                .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                    nameof(UpdateEthicsPrincipleDto.Text), HippotherapyLandingPageConstants.EthicsPrincipleMaxLength));
        });

        RuleFor(x => x.ImageId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateEthicsSectionDto.ImageId)))
            .When(x => x.ImageId.HasValue);
    }
}
