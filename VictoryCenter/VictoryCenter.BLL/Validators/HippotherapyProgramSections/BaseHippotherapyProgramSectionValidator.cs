using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;

namespace VictoryCenter.BLL.Validators.HippotherapyProgramSections;

public class BaseHippotherapyProgramSectionValidator
    : AbstractValidator<CreateHippotherapyProgramSectionDto>
{
    public BaseHippotherapyProgramSectionValidator()
    {
        RuleFor(x => x.Template)
            .IsInEnum()
            .WithMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(
                nameof(CreateHippotherapyProgramSectionDto.Template)));

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThan(
                nameof(CreateHippotherapyProgramSectionDto.Order), -1));

        RuleFor(x => x.Titles)
            .Must(HasValidTitlesCount)
            .WithMessage(ProgramSectionConstants.GetTitlesCountErrorMessage)
            .When(HasKnownTemplate)
            .DependentRules(() =>
            {
                RuleForEach(x => x.Titles)
                    .NotEmpty()
                    .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateHippotherapyProgramSectionDto.Titles)))
                    .Must(HasValidTitleLength)
                    .WithMessage(ProgramSectionConstants.GetTitleLengthErrorMessage);
            });

        RuleFor(x => x.Descriptions)
            .Must(HasValidDescriptionsCount)
            .WithMessage(ProgramSectionConstants.GetDescriptionsCountErrorMessage)
            .When(HasKnownTemplate)
            .DependentRules(() =>
            {
                RuleForEach(x => x.Descriptions)
                    .NotEmpty()
                    .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateHippotherapyProgramSectionDto.Descriptions)))
                    .Must(HasValidDescriptionLength)
                    .WithMessage(ProgramSectionConstants.GetDescriptionLengthErrorMessage);
            });

        RuleFor(x => x.ImageIds)
            .Must(HasValidImagesCount)
            .WithMessage(ProgramSectionConstants.GetImagesCountErrorMessage)
            .When(HasKnownTemplate)
            .Must(imageIds => imageIds is null || imageIds.Distinct().Count() == imageIds.Count)
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(
                nameof(CreateHippotherapyProgramSectionDto.ImageIds)))
            .DependentRules(() =>
            {
                RuleForEach(x => x.ImageIds)
                    .GreaterThan(0)
                    .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateHippotherapyProgramSectionDto.ImageIds)));
            });
    }

    private static bool HasKnownTemplate(CreateHippotherapyProgramSectionDto section) =>
        ProgramSectionConstants.TemplateRequirements.ContainsKey(section.Template);

    private static ProgramSectionConstants.TemplateRequirementsConfig GetReq(CreateHippotherapyProgramSectionDto section) =>
        ProgramSectionConstants.TemplateRequirements[section.Template];

    private static bool HasValidTitlesCount(CreateHippotherapyProgramSectionDto section, List<string>? titles) =>
        HasValidCount(titles, GetReq(section).TitleCount);

    private static bool HasValidDescriptionsCount(CreateHippotherapyProgramSectionDto section, List<string>? descriptions) =>
        HasValidCount(descriptions, GetReq(section).DescriptionCount);

    private static bool HasValidImagesCount(CreateHippotherapyProgramSectionDto section, List<long>? imageIds) =>
        HasValidCount(imageIds, GetReq(section).ImageCount);

    private static bool HasValidTitleLength(CreateHippotherapyProgramSectionDto section, string? title) =>
        HasValidLength(title, GetReq(section).TitleLength);

    private static bool HasValidDescriptionLength(CreateHippotherapyProgramSectionDto section, string? description) =>
        HasValidLength(description, GetReq(section).DescriptionLength);

    private static bool HasValidCount<T>(List<T>? collection, (int Min, int Max) req)
    {
        var count = collection?.Count ?? 0;
        if (req.Min == 0 && req.Max == 0)
        {
            return count == 0;
        }

        return collection is not null
               && count >= req.Min
               && count <= req.Max;
    }

    private static bool HasValidLength(string? text, (int Min, int Max) lengthRequirements)
    {
        var trimmed = text?.Trim();

        return !string.IsNullOrEmpty(trimmed)
               && trimmed.Length >= lengthRequirements.Min
               && trimmed.Length <= lengthRequirements.Max;
    }
}
