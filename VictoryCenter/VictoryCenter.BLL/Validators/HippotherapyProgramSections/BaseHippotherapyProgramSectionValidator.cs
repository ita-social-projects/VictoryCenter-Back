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
            .DependentRules(() =>
            {
                RuleForEach(x => x.Titles)
                    .NotEmpty()
                    .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Title"))
                    .Must(HasValidTitleLength)
                    .WithMessage(ProgramSectionConstants.GetTitleLengthErrorMessage);
            });

        RuleFor(x => x.Descriptions)
            .Must(HasValidDescriptionsCount)
            .WithMessage(ProgramSectionConstants.GetDescriptionsCountErrorMessage)
            .DependentRules(() =>
            {
                RuleForEach(x => x.Descriptions)
                    .NotEmpty()
                    .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Description"))
                    .Must(HasValidDescriptionLength)
                    .WithMessage(ProgramSectionConstants.GetDescriptionLengthErrorMessage);
            });

        RuleFor(x => x.ImageIds)
            .Must(HasValidImagesCount)
            .WithMessage(ProgramSectionConstants.GetImagesCountErrorMessage)
            .Must(imageIds => imageIds is null || imageIds.Distinct().Count() == imageIds.Count)
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(
                nameof(CreateHippotherapyProgramSectionDto.ImageIds)))
            .DependentRules(() =>
            {
                RuleForEach(x => x.ImageIds)
                    .GreaterThan(0)
                    .WithMessage(ErrorMessagesConstants.PropertyMustBePositive("ImageId"));
            });
    }

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

    private static bool HasValidCount<T>(List<T>? collection, (int Min, int Max) countRequirements)
    {
        var actualCount = collection?.Count ?? 0;
        var requiredCount = countRequirements.Min;

        return requiredCount == 0
            ? actualCount == 0
            : collection is not null && actualCount == requiredCount;
    }

    private static bool HasValidLength(string? text, (int Min, int Max) lengthRequirements) =>
        !string.IsNullOrWhiteSpace(text) &&
        text.Length >= lengthRequirements.Min &&
        text.Length <= lengthRequirements.Max;
}
