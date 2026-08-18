using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.HippotherapyLandingPage.Dto;

public class UpdateGallerySectionDtoValidator : AbstractValidator<UpdateGallerySectionDto>
{
    public UpdateGallerySectionDtoValidator(UpdateGalleryCardDtoValidator cardValidator)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateGallerySectionDto.Title)))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length >= HippotherapyLandingPageConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateGallerySectionDto.Title), HippotherapyLandingPageConstants.TitleMinLength))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length <= HippotherapyLandingPageConstants.GalleryTitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateGallerySectionDto.Title), HippotherapyLandingPageConstants.GalleryTitleMaxLength));

        RuleFor(x => x.Cards)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateGallerySectionDto.Cards)))
            .Must(c => c.Count == HippotherapyLandingPageConstants.GalleryCardsCount)
            .WithMessage(ErrorMessagesConstants.CollectionMustContainExactlyNItems(
                nameof(UpdateGallerySectionDto.Cards), HippotherapyLandingPageConstants.GalleryCardsCount));

        RuleForEach(x => x.Cards).SetValidator(cardValidator);
    }
}
