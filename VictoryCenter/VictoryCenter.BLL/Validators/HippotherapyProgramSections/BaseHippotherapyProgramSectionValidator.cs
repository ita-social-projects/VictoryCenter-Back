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

        When(x => ProgramSectionConstants.TemplateRequirements.ContainsKey(x.Template), () =>
        {
            RuleFor(x => x.Titles)
                .Must((dto, titles) => IsValidCount(titles, GetRequirements(dto).TitleCount))
                .WithMessage(x => ProgramSectionConstants.TemplateRequiresExactlyNTitles(
                    x.Template,
                    GetRequirements(x).TitleCount.Min,
                    x.Titles?.Count ?? 0));

            RuleFor(x => x.Descriptions)
                .Must((dto, descriptions) => IsValidCount(descriptions, GetRequirements(dto).DescriptionCount))
                .WithMessage(x => ProgramSectionConstants.TemplateRequiresExactlyNDescriptions(
                    x.Template,
                    GetRequirements(x).DescriptionCount.Min,
                    x.Descriptions?.Count ?? 0));

            RuleFor(x => x.ImageIds)
                .Must((dto, imageIds) => IsValidCount(imageIds, GetRequirements(dto).ImageCount))
                .WithMessage(x => ProgramSectionConstants.TemplateRequiresExactlyNImages(
                    x.Template,
                    GetRequirements(x).ImageCount.Min,
                    x.ImageIds?.Count ?? 0))
                .Must(list => list is null || list.Distinct().Count() == list.Count)
                .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(
                    nameof(CreateHippotherapyProgramSectionDto.ImageIds)));

            RuleForEach(x => x.Titles)
                .NotEmpty()
                .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Title"))
                .Must((dto, title) => IsValidLength(title, GetRequirements(dto).TitleLength))
                .WithMessage(x => ProgramSectionConstants.TitleMustBeBetweenNAndMCharacters(
                    GetRequirements(x).TitleLength.Min,
                    GetRequirements(x).TitleLength.Max));

            RuleForEach(x => x.Descriptions)
                .NotEmpty()
                .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Description"))
                .Must((dto, description) => IsValidLength(description, GetRequirements(dto).DescriptionLength))
                .WithMessage(x => ProgramSectionConstants.DescriptionMustBeBetweenNAndMCharacters(
                    GetRequirements(x).DescriptionLength.Min,
                    GetRequirements(x).DescriptionLength.Max));

            RuleForEach(x => x.ImageIds)
                .GreaterThan(0)
                .WithMessage(ErrorMessagesConstants.PropertyMustBePositive("ImageId"));
        });
    }

    private static (
        (int Min, int Max) TitleCount,
        (int Min, int Max) TitleLength,
        (int Min, int Max) DescriptionCount,
        (int Min, int Max) DescriptionLength,
        (int Min, int Max) ImageCount
    ) GetRequirements(CreateHippotherapyProgramSectionDto dto) =>
        ProgramSectionConstants.TemplateRequirements[dto.Template];

    private static bool IsValidCount<T>(List<T>? items, (int Min, int Max) requirements)
    {
        var actualCount = items?.Count ?? 0;
        var requiredCount = requirements.Min;

        return requiredCount == 0 ? actualCount == 0 : items is not null && actualCount == requiredCount;
    }

    private static bool IsValidLength(string? text, (int Min, int Max) requirements) =>
        !string.IsNullOrWhiteSpace(text) &&
        text.Length >= requirements.Min &&
        text.Length <= requirements.Max;
}
