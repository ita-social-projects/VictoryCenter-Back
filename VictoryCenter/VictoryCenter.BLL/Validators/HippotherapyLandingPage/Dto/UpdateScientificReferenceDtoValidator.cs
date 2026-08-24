using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;

namespace VictoryCenter.BLL.Validators.HippotherapyLandingPage.Dto;

public class UpdateScientificReferenceDtoValidator : AbstractValidator<UpdateScientificReferenceDto>
{
    public UpdateScientificReferenceDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateScientificReferenceDto.Name)))
            .MinimumLength(HippotherapyLandingPageConstants.ScientificReferenceNameMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateScientificReferenceDto.Name), HippotherapyLandingPageConstants.ScientificReferenceNameMinLength))
            .MaximumLength(HippotherapyLandingPageConstants.ScientificReferenceNameMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateScientificReferenceDto.Name), HippotherapyLandingPageConstants.ScientificReferenceNameMaxLength));

        RuleFor(x => x.Url)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateScientificReferenceDto.Url)))
            .MinimumLength(HippotherapyLandingPageConstants.ScientificReferenceUrlMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateScientificReferenceDto.Url), HippotherapyLandingPageConstants.ScientificReferenceUrlMinLength))
            .MaximumLength(HippotherapyLandingPageConstants.ScientificReferenceUrlMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateScientificReferenceDto.Url), HippotherapyLandingPageConstants.ScientificReferenceUrlMaxLength))
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(nameof(UpdateScientificReferenceDto.Url)))
            .When(x => !string.IsNullOrWhiteSpace(x.Url), ApplyConditionTo.CurrentValidator);
    }
}
