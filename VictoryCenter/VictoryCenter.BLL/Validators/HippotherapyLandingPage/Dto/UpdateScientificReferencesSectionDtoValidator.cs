using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.HippotherapyLandingPage.Dto;

public class UpdateScientificReferencesSectionDtoValidator : AbstractValidator<UpdateScientificReferencesSectionDto>
{
    public UpdateScientificReferencesSectionDtoValidator(UpdateScientificReferenceDtoValidator referenceValidator)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateScientificReferencesSectionDto.Title)))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length >= HippotherapyLandingPageConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateScientificReferencesSectionDto.Title), HippotherapyLandingPageConstants.TitleMinLength))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length <= HippotherapyLandingPageConstants.ScientificReferencesTitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateScientificReferencesSectionDto.Title), HippotherapyLandingPageConstants.ScientificReferencesTitleMaxLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateScientificReferencesSectionDto.Description)))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length >= HippotherapyLandingPageConstants.TextMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateScientificReferencesSectionDto.Description), HippotherapyLandingPageConstants.TextMinLength))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length <= HippotherapyLandingPageConstants.ScientificReferencesDescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateScientificReferencesSectionDto.Description), HippotherapyLandingPageConstants.ScientificReferencesDescriptionMaxLength));

        RuleFor(x => x.ScientificReferences)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateScientificReferencesSectionDto.ScientificReferences)))
            .Must(r => r.Count > 0)
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(UpdateScientificReferencesSectionDto.ScientificReferences)))
            .Must(r => r.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).Distinct().Count() ==
                       r.Count(x => x.Id.HasValue))
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(
                nameof(UpdateScientificReferencesSectionDto.ScientificReferences)));

        RuleForEach(x => x.ScientificReferences).SetValidator(referenceValidator);
    }
}
