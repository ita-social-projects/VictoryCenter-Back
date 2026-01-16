using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Validators.HippotherapyProgramSections;

public class BaseHippotherapyProgramSectionValidator : AbstractValidator<CreateHippotherapyProgramSectionDto>
{
    public BaseHippotherapyProgramSectionValidator()
    {
        RuleFor(x => x.Template)
            .IsInEnum()
            .WithMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(CreateHippotherapyProgramSectionDto.Template)));

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThan(nameof(CreateHippotherapyProgramSectionDto.Order), -1));

        RuleFor(x => x.Contents)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateHippotherapyProgramSectionDto.Contents)));

        RuleFor(x => x).Custom(ValidateSection);
    }

    private static void ValidateSection(
        CreateHippotherapyProgramSectionDto section,
        ValidationContext<CreateHippotherapyProgramSectionDto> ctx)
    {
        if (section.Contents is null)
        {
            return;
        }

        if (!Enum.IsDefined(section.Template))
        {
            return;
        }

        var contents = section.Contents;

        var req = HippotherapyProgramSectionConstants.GetRequirements(section.Template);
        const string? prop = nameof(CreateHippotherapyProgramSectionDto.Contents);

        ValidateCounts(section, ctx, contents, req, prop);
        ValidateUniqueness(ctx, contents, prop);
        ValidateBasicValues(ctx, contents, prop);
        ValidateTitles(section, ctx, contents, req, prop);
        ValidateDescriptions(section, ctx, contents, req, prop);
        ValidateImages(ctx, contents, prop);
        ValidateAuthors(section, ctx, contents, req, prop);
        ValidateGrouping(section, ctx, contents, req, prop);
    }

    private static void ValidateCounts(
        CreateHippotherapyProgramSectionDto section,
        ValidationContext<CreateHippotherapyProgramSectionDto> ctx,
        List<CreateProgramSectionContentDto> contents,
        HippotherapyProgramSectionConstants.TemplateRequirementsConfig req,
        string prop)
    {
        if (!InRange(CountByType(contents, ContentType.Title), req.TitleCount))
        {
            ctx.AddFailure(prop, HippotherapyProgramSectionConstants.GetTitlesCountErrorMessage(section));
        }

        if (!InRange(CountByType(contents, ContentType.Description), req.DescriptionCount))
        {
            ctx.AddFailure(prop, HippotherapyProgramSectionConstants.GetDescriptionsCountErrorMessage(section));
        }

        if (!InRange(CountByType(contents, ContentType.Image), req.ImageCount))
        {
            ctx.AddFailure(prop, HippotherapyProgramSectionConstants.GetImagesCountErrorMessage(section));
        }

        if (!InRange(CountByType(contents, ContentType.Author), req.AuthorCount))
        {
            ctx.AddFailure(prop, HippotherapyProgramSectionConstants.GetAuthorsCountErrorMessage(section));
        }
    }

    private static void ValidateUniqueness(
        ValidationContext<CreateHippotherapyProgramSectionDto> ctx,
        List<CreateProgramSectionContentDto> contents,
        string prop)
    {
        if (!HasUniqueOrders(contents))
        {
            ctx.AddFailure(prop, ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(CreateProgramSectionContentDto.Order)));
        }

        if (!HasUniqueImageIds(contents))
        {
            ctx.AddFailure(prop, ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(CreateProgramSectionContentDto.ImageId)));
        }
    }

    private static void ValidateBasicValues(
        ValidationContext<CreateHippotherapyProgramSectionDto> ctx,
        List<CreateProgramSectionContentDto> contents,
        string prop)
    {
        if (contents.Any(c => !Enum.IsDefined(c.ContentType)))
        {
            ctx.AddFailure(prop, ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(CreateProgramSectionContentDto.ContentType)));
        }

        if (contents.Any(c => c.Order < 0))
        {
            ctx.AddFailure(prop, ErrorMessagesConstants.PropertyMustBeGreaterThan(nameof(CreateProgramSectionContentDto.Order), -1));
        }

        if (contents.Any(c => c.GroupIndex is < 0))
        {
            ctx.AddFailure(prop, ErrorMessagesConstants.PropertyMustBeGreaterThan(nameof(CreateProgramSectionContentDto.GroupIndex), -1));
        }
    }

    private static void ValidateTitles(
        CreateHippotherapyProgramSectionDto section,
        ValidationContext<CreateHippotherapyProgramSectionDto> ctx,
        List<CreateProgramSectionContentDto> contents,
        HippotherapyProgramSectionConstants.TemplateRequirementsConfig req,
        string prop)
    {
        var titles = contents.Where(c => c.ContentType == ContentType.Title).ToList();

        if (titles.Any(c => string.IsNullOrWhiteSpace(c.Title)))
        {
            ctx.AddFailure(prop, ErrorMessagesConstants.PropertyIsRequired(nameof(CreateProgramSectionContentDto.Title)));
        }

        if (titles.Any(c => !HasValidLength(c.Title, req.TitleLength)))
        {
            ctx.AddFailure(prop, HippotherapyProgramSectionConstants.GetTitleLengthErrorMessage(section));
        }
    }

    private static void ValidateDescriptions(
        CreateHippotherapyProgramSectionDto section,
        ValidationContext<CreateHippotherapyProgramSectionDto> ctx,
        List<CreateProgramSectionContentDto> contents,
        HippotherapyProgramSectionConstants.TemplateRequirementsConfig req,
        string prop)
    {
        var descriptions = contents.Where(c => c.ContentType == ContentType.Description).ToList();

        if (descriptions.Any(c => string.IsNullOrWhiteSpace(c.Description)))
        {
            ctx.AddFailure(prop, ErrorMessagesConstants.PropertyIsRequired(nameof(CreateProgramSectionContentDto.Description)));
        }

        if (descriptions.Any(c => !HasValidLength(c.Description, req.DescriptionLength)))
        {
            ctx.AddFailure(prop, HippotherapyProgramSectionConstants.GetDescriptionLengthErrorMessage(section));
        }
    }

    private static void ValidateImages(
        ValidationContext<CreateHippotherapyProgramSectionDto> ctx,
        List<CreateProgramSectionContentDto> contents,
        string prop)
    {
        var images = contents.Where(c => c.ContentType == ContentType.Image).ToList();

        if (images.Any(c => c.ImageId is null or <= 0))
        {
            ctx.AddFailure(prop, ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateProgramSectionContentDto.ImageId)));
        }
    }

    private static void ValidateAuthors(
        CreateHippotherapyProgramSectionDto section,
        ValidationContext<CreateHippotherapyProgramSectionDto> ctx,
        List<CreateProgramSectionContentDto> contents,
        HippotherapyProgramSectionConstants.TemplateRequirementsConfig req,
        string prop)
    {
        if (req.AuthorCount.Min == 0 && req.AuthorCount.Max == 0)
        {
            return;
        }

        var authors = contents.Where(c => c.ContentType == ContentType.Author).ToList();

        if (authors.Any(c => string.IsNullOrWhiteSpace(c.Author)))
        {
            ctx.AddFailure(prop, ErrorMessagesConstants.PropertyIsRequired(nameof(CreateProgramSectionContentDto.Author)));
        }

        if (authors.Any(c => !HasValidLength(c.Author, req.AuthorLength)))
        {
            ctx.AddFailure(prop, HippotherapyProgramSectionConstants.GetAuthorLengthErrorMessage(section));
        }
    }

    private static void ValidateGrouping(
        CreateHippotherapyProgramSectionDto section,
        ValidationContext<CreateHippotherapyProgramSectionDto> ctx,
        List<CreateProgramSectionContentDto> contents,
        HippotherapyProgramSectionConstants.TemplateRequirementsConfig req,
        string prop)
    {
        var grouping = req.Grouping;
        if (grouping is null)
        {
            return;
        }

        var grouped = contents.Where(c => grouping.PerGroupCounts.ContainsKey(c.ContentType)).ToList();

        if (grouped.Any(c => c.GroupIndex is null))
        {
            ctx.AddFailure(prop, HippotherapyProgramSectionConstants.GetGroupIndexRequiredErrorMessage(section));
            return;
        }

        var groups = grouped.GroupBy(c => c.GroupIndex!.Value).ToList();

        if (!InRange(groups.Count, grouping.GroupCount))
        {
            ctx.AddFailure(prop, HippotherapyProgramSectionConstants.GetGroupCountErrorMessage(section, groups.Count));
            return;
        }

        if (!GroupsMatchComposition(grouped, grouping))
        {
            ctx.AddFailure(prop, HippotherapyProgramSectionConstants.GetGroupCompositionErrorMessage(section));
        }
    }

    private static int CountByType(
        List<CreateProgramSectionContentDto> contents,
        ContentType type)
        => contents.Count(c => c.ContentType == type);

    private static bool HasUniqueOrders(List<CreateProgramSectionContentDto> contents)
        => contents.Select(c => c.Order).Distinct().Count() == contents.Count;

    private static bool HasUniqueImageIds(List<CreateProgramSectionContentDto> contents)
    {
        var ids = contents
            .Where(c => c.ContentType == ContentType.Image && c.ImageId.HasValue)
            .Select(c => c.ImageId!.Value)
            .ToList();

        return ids.Distinct().Count() == ids.Count;
    }

    private static bool GroupsMatchComposition(
        IEnumerable<CreateProgramSectionContentDto> contents,
        HippotherapyProgramSectionConstants.GroupingConfig grouping)
    {
        return !contents
            .Where(c => grouping.PerGroupCounts.ContainsKey(c.ContentType))
            .GroupBy(c => c.GroupIndex!.Value)
            .Any(g => grouping.PerGroupCounts.Any(rule =>
                !InRange(g.Count(x => x.ContentType == rule.Key), rule.Value)));
    }

    private static bool InRange(int actual, (int Min, int Max) req)
        => (req.Min == 0 && req.Max == 0) ? actual == 0 : actual >= req.Min && actual <= req.Max;

    private static bool HasValidLength(string? text, (int Min, int Max) req)
    {
        var t = text?.Trim();
        return !string.IsNullOrEmpty(t) && t.Length >= req.Min && t.Length <= req.Max;
    }
}
